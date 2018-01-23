﻿using System.Collections.Generic;
using System.IO;
using CommonTools.Log;
using ComponentModel.Common;
using MathematicsHelper;
using Microsoft.Scripting.Hosting;
using Physics.Box2D;
using PythonScripting;
using Shared.Game.Common;

namespace Game.Application.Components
{
    public class SceneContainer : Component, ISceneContainer
    {
        private readonly Dictionary<Maps, IGameSceneWrapper> scenes = new Dictionary<Maps, IGameSceneWrapper>();

        private IPythonScriptEngine pythonScriptEngine;
        private ScriptScope scriptScope;

        protected override void OnAwake()
        {
            base.OnAwake();

            pythonScriptEngine = Entity.GetComponent<IPythonScriptEngine>().AssertNotNull();

            scriptScope = pythonScriptEngine.GetScriptEngine().CreateScope();
            scriptScope.SetVariable("sceneContainer", this);

            AddScenesViaPython();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var scenesTemp = new List<IGameSceneWrapper>();
            scenesTemp.AddRange(scenes.Values);

            scenes.Clear();

            foreach (var gameSceneWrapper in scenesTemp)
            {
                gameSceneWrapper.GetScene().Dispose();
            }
        }

        private void AddScenesViaPython()
        {
            const string PYTHON_SCRIPTS_PATH = "python/scenes";
            var pythonScripts = Directory.GetFiles(PYTHON_SCRIPTS_PATH, "*.py", SearchOption.TopDirectoryOnly);

            foreach (var pythonScript in pythonScripts)
            {
                pythonScriptEngine.GetScriptEngine().ExecuteFile(pythonScript, scriptScope);
            }
        }

        public void AddScene(Maps map, Vector2 sceneSize, Vector2 regionSize, PhysicsWorldInfo physicsWorldInfo, bool drawPhysics)
        {
            var gameSceneWrapper = new GameSceneWrapper(map, sceneSize, regionSize);
            scenes.Add(map, gameSceneWrapper);

            gameSceneWrapper.GetScene().Entity.AddComponent(new SceneOrderExecutor());
            gameSceneWrapper.GetScene().Entity.AddComponent(new PhysicsSimulationCreator(physicsWorldInfo));
            gameSceneWrapper.GetScene().Entity.AddComponent(new PhysicsMapCreator(map));
            gameSceneWrapper.GetScene().Entity.AddComponent(new EntityManager());
            gameSceneWrapper.CreateSceneObjectsViaPython();

            if (drawPhysics)
            {
                gameSceneWrapper.GetScene().Entity.AddComponent(new PhysicsSimulationWindowCreator(map.ToString()));
            }
        }

        public IGameSceneWrapper GetSceneWrapper(Maps map)
        {
            if (scenes.TryGetValue(map, out var scene))
            {
                return scene;
            }

            LogUtils.Log(MessageBuilder.Trace($"Could not find a scene with map {map}"), LogMessageType.Error);
            return null;
        }
    }
}
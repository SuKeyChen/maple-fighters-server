﻿using MathematicsHelper;
using SceneObject = Game.InterestManagement.SceneObject;

namespace Game.Application.SceneObjects
{
    public class Portal : SceneObject
    {
        private const string SCENE_OBJECT_NAME = "Portal";

        public Portal(Vector2 position, Vector2 playerPosition, int map) 
            : base(SCENE_OBJECT_NAME, position)
        {
            Container.AddComponent(new PortalInfoProvider(playerPosition, map));
        }
    }
}
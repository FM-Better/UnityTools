using UnityEngine;

namespace Utility
{
    public static class TransformExtension
    {
        public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
        {
            var selfComponentTransform = selfComponent.transform;
            var localScale = selfComponentTransform.localScale;
            localScale.x = x;
            selfComponentTransform.localScale = localScale;
            return selfComponent;
        }
        
        public static T LocalScaleY<T>(this T selfComponent, float y) where T : Component
        {
            var selfComponentTransform = selfComponent.transform;
            var localScale = selfComponentTransform.localScale;
            localScale.y = y;
            selfComponentTransform.localScale = localScale;
            return selfComponent;
        }
        
        public static T LocalScaleZ<T>(this T selfComponent, float z) where T : Component
        {
            var selfComponentTransform = selfComponent.transform;
            var localScale = selfComponentTransform.localScale;
            localScale.z = z;
            selfComponentTransform.localScale = localScale;
            return selfComponent;
        }
        
        public static T PositionX<T>(this T selfComponent, float x) where T : Component
        {
            var selfComponentTransform = selfComponent.transform;
            var position = selfComponentTransform.position;
            position.x = x;
            selfComponentTransform.position = position;
            return selfComponent;
        }
        
        public static T PositionY<T>(this T selfComponent, float y) where T : Component
        {
            var selfComponentTransform = selfComponent.transform;
            var position = selfComponentTransform.position;
            position.y = y;
            selfComponentTransform.position = position;
            return selfComponent;
        }
        
        public static T PositionZ<T>(this T selfComponent, float z) where T : Component
        {
            var selfComponentTransform = selfComponent.transform;
            var position = selfComponentTransform.position;
            position.z = z;
            selfComponentTransform.position = position;
            return selfComponent;
        }
    }
}
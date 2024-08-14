using UnityEngine;

namespace Utility
{
    /// <summary>
    /// 设置所挂载的渲染器的渲染顺序 (SortingLayer、SortingOrder
    /// </summary>
    public class SetRenderOrder : MonoBehaviour
    {
        [SerializeField] private SortingLayerMask  _sortingLayer;
        [SerializeField] private int _sortingOrder;
    
        private void Start()
        {
            ApplyRenderOrder();
        }

        public void ApplyRenderOrder()
        {
            var selfRenderer = GetComponent<Renderer>(); 
            selfRenderer.sortingLayerID = _sortingLayer.SortingLayerID;
            selfRenderer.sortingOrder = _sortingOrder;
        }
    }
    
    [System.Serializable]
    public struct SortingLayerMask
    {
        public int SortingLayerID;
        public string SortingLayerName;

        public SortingLayerMask(int layerID, string layerName)
        {
            SortingLayerID = layerID;
            SortingLayerName = layerName;
        }
    }
}

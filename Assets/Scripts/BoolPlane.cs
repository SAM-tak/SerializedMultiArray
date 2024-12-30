using System;
using UnityEngine;

namespace SerializedArray
{
    [Serializable]
    public class BoolPlane : ISerializationCallbackReceiver
    {
        /// <summary>
        /// 配列の値
        /// </summary>
        public bool[,] Value { get; set; }

        /// <summary>
        /// 配列の横(X軸)
        /// </summary>
        public int Width => _size.x;

        /// <summary>
        /// 配列の高さ(Y軸)
        /// </summary>
        public int Height => _size.y;

        // 値保存用
        [SerializeField, HideInInspector]
        bool[] _store;

        // 配列のサイズ
        [HideInInspector, SerializeField]
        Vector2Int _size;

        public void OnBeforeSerialize()
        {
            if(Value == null) {
                return;
            }

            var w = Value.GetLength(0);
            var h = Value.GetLength(1);

            // 要素数が違えばリサイズする
            if(_store == null || w * h != Width * Height) {
                _store = new bool[w * h];
            }

            _size.x = w;
            _size.y = h;

            // 要素をコピー
            for(int y = 0; y < h; y++) {
                for(int x = 0; x < w; x++) {
                    _store[x + w * y] = Value[x, y];
                }
            }
        }

        public void OnAfterDeserialize()
        {
            // Valueがnullかサイズが違う場合、リサイズする
            if(Value != null) {
                var w = Value.GetLength(0);
                var h = Value.GetLength(1);

                if(Width != w || Height != h) {
                    Value = new bool[Width, Height];
                }
            }
            else {
                Value = new bool[Width, Height];
            }

            // 要素をコピー
            for(int y = 0; y < Height; y++) {
                for(int x = 0; x < Width; x++) {
                    Value[x, y] = _store[x + Width * y];
                }
            }
            _store = null;
        }
    }
}

/*Docs
 *https://docs.unity3d.com/ja/2021.2/Manual/script-Serialization-Custom.html
 */

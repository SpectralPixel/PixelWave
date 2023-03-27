using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTextureLoader : MonoBehaviour
{
    [System.Serializable]
    public class TCubeTexture
    {
        public string TextureName;

        public Sprite XTexture, YTexture, ZTexture;

        public TFaceTextures SpecificFaceTextures;

        [System.Serializable]
        public class TFaceTextures
        {
            public Sprite Up, Down;
            [Space]
            public Sprite Left, Right;
            [Space]
            public Sprite Forward, Back;
        }

        public Vector2[] GetUVsAtDirection(Vector3Int _direction)
        {
            if (_direction == Vector3Int.forward) return ZTexture.uv != null ? ZTexture.uv : SpecificFaceTextures.Forward.uv;
            else if (_direction == Vector3Int.back) return ZTexture.uv != null ? ZTexture.uv : SpecificFaceTextures.Back.uv;

            if (_direction == Vector3Int.right) return XTexture.uv != null ? XTexture.uv : SpecificFaceTextures.Right.uv;
            else if (_direction == Vector3Int.left) return XTexture.uv != null ? XTexture.uv : SpecificFaceTextures.Left.uv;

            if (_direction == Vector3Int.up) return YTexture.uv != null ? YTexture.uv : SpecificFaceTextures.Up.uv;
            else if (_direction == Vector3Int.down) return YTexture.uv != null ? YTexture.uv : SpecificFaceTextures.Down.uv;

            return null;
        }
    }

    [SerializeField] private TCubeTexture[] cubeTextures;
    public Dictionary<int, TCubeTexture> Textures;

    private void Awake()
    {
        Textures = new Dictionary<int, TCubeTexture>();

        for (int i = 0; i < cubeTextures.Length; i++)
        {
            Textures.Add(i + 1, cubeTextures[i]);
        }
    }
}

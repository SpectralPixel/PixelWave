using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLoader : MonoBehaviour
{
    [System.Serializable]
    public class CubeTexture
    {
        public string TextureName;

        public Sprite xTexture, yTexture, zTexture;

        public FaceTextures SpecificFaceTextures;

        [System.Serializable]
        public class FaceTextures
        {
            public Sprite Up, Down;
            [Space]
            public Sprite Left, Right;
            [Space]
            public Sprite Forward, Back;
        }

        public Vector2[] GetUVsAtDirection(Vector3Int _direction)
        {
            if (_direction == Vector3Int.forward) return zTexture.uv != null ? zTexture.uv : SpecificFaceTextures.Forward.uv;
            else if (_direction == Vector3Int.back) return zTexture.uv != null ? zTexture.uv : SpecificFaceTextures.Back.uv;

            if (_direction == Vector3Int.right) return xTexture.uv != null ? xTexture.uv : SpecificFaceTextures.Right.uv;
            else if (_direction == Vector3Int.left) return xTexture.uv != null ? xTexture.uv : SpecificFaceTextures.Left.uv;

            if (_direction == Vector3Int.up) return yTexture.uv != null ? yTexture.uv : SpecificFaceTextures.Up.uv;
            else if (_direction == Vector3Int.down) return yTexture.uv != null ? yTexture.uv : SpecificFaceTextures.Down.uv;

            return null;
        }
    }

    [SerializeField] private CubeTexture[] cubeTextures;
    public Dictionary<int, CubeTexture> Textures;

    private void Awake()
    {
        Textures = new Dictionary<int, CubeTexture>();

        for (int i = 0; i < cubeTextures.Length; i++)
        {
            Textures.Add(i + 1, cubeTextures[i]);
        }
    }
}

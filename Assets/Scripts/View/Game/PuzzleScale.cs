using Core.Data;
using UnityEngine;
using View.Control;

namespace View.Game
{
    public class PuzzleScale : MonoBehaviour
    {
        // Constants
        public float Scaling => GameDef.Get.Scaling;

        public float NodeScaling => GameDef.Get.NodeScaling;
        public float EdgeScaling => GameDef.Get.EdgeScaling;
        public float BoardScaling => GameDef.Get.BoardScaling;
        public float BoardPadding => GameDef.Get.BoardPadding;
        public Vector3 BoardRotation => GameDef.Get.BoardRotation;

        public Vector2 Dimensions { get; private set; }

        public Vector2 Offset { get; private set; }

        public Vector2 MinClamp { get; private set; }
        public Vector2 MaxClamp { get; private set; }

        public Vector2 CameraDimensions { get; private set; }

        private void Awake()
        {
            Get = this;

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Init(Point startNode, Point boardSize)
        {
            CameraDimensions = GetCameraDimensions();
            Dimensions = new Vector2(boardSize.X, boardSize.Y)*Scaling;
            GetClamp();

            transform.localEulerAngles = BoardRotation;

            // Move the board to the center of the screen
            Offset = -Dimensions / 2f;
            LeanTween.moveLocal(gameObject, Offset, 1f)
                .setEase(LeanTweenType.easeInOutSine);

            // Calculate dimensions of the game board + a small margin to prevent cutoff around the edges,
            // then calculate a scaled zoom value based on the ratio of the board dimensions to the camera dimensions
            // so that the board never gets cut off by the camera
            var margin = NodeScaling * new Vector2(2f, 3f);
            var scaledDimensions = Dimensions + margin;
            var cameraZoomScale = new Vector2(scaledDimensions.x / CameraDimensions.x, scaledDimensions.y / CameraDimensions.y);
            var cameraZoom = Camera.main.orthographicSize * cameraZoomScale;
            var maxZoom = Mathf.Max(cameraZoom.x, cameraZoom.y);

            LeanTween.value(Camera.main.orthographicSize, maxZoom, GameDef.Get.WaveInMoveDelayStart)
                .setEase(LeanTweenType.easeInOutSine)
                .setDelay(GameDef.Get.LevelDelay)
                .setOnUpdate(scaled => Camera.main.orthographicSize = scaled);
        }

        private void GetClamp()
        {
            var margin = NodeScaling * Vector2.one; // Add margin to prevent node cutoff

            var minClamp = CameraDimensions / 2f - Dimensions - margin;
            var maxClamp = -CameraDimensions / 2f + margin;

            if (minClamp.x > maxClamp.x) {
                minClamp.x = maxClamp.x; 
            }
            if (minClamp.y > maxClamp.y) {
                minClamp.y = maxClamp.y = 0;
            }

            MinClamp = minClamp;
            MaxClamp = maxClamp;
        }

        private static Vector2 GetCameraDimensions()
        {
            var cam = Camera.main;
            var p1 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            var p2 = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            var p3 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            var width = (p2 - p1).magnitude;
            var height = (p3 - p2).magnitude;

            return new Vector2(width, height);
        }

        public Vector2 Scale(Vector2 boardPos)
        {
            return boardPos*Scaling;
        }

        public Vector2 Clamp(Vector2 pos)
        {
            return new Vector2(
                Mathf.Clamp(pos.x, MinClamp.x, MaxClamp.x),
                Mathf.Clamp(pos.y, MinClamp.y, MaxClamp.y)
            );
        }

        public static PuzzleScale Get { get; private set; }
    }
}

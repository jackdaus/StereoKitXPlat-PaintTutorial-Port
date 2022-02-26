using StereoKit;
using System.Numerics;

namespace StereoKitApp
{
	public class App
	{
		public SKSettings Settings => new SKSettings { 
			appName           = "StereoKit Template",
			assetsFolder      = "Assets",
			displayPreference = DisplayMode.MixedReality
		};

		static float cubeX = 0.0f;
		Pose  cubePose = new Pose(cubeX, 0, -0.5f, Quat.Identity);
		Model cube;
		Matrix4x4 floorTransform = Matrix.TS(new Vector3(0, -1.5f, 0), new Vector3(30, 0.1f, 30));
		Material  floorMaterial;

		// UI param initialization
		// Sprite powerSprite = Sprite.FromFile("power.png", SpriteType.Single);
		Pose windowPose = new Pose(-.2f, .1f, -0.2f, Quat.LookDir(1, 0, 1));
		bool showHeader = true;
		bool moveCube = true;
		float slider = 0.001f;

		public void Init()
		{
			// Create assets used by the app
			cube = Model.FromMesh(
				Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
				Default.MaterialUI);

			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;
		}

		public void Step()
		{
			if (SK.System.displayType == Display.Opaque)
				Default.MeshCube.Draw(floorMaterial, floorTransform);

			// Update the cube location if roaming
			var newCubePositionX = cubePose.position.x;
			if (moveCube)
			{
				newCubePositionX += slider;
			}
			cubePose = new Pose(newCubePositionX, cubePose.position.y, cubePose.position.z, cubePose.orientation);

			UI.Handle("Cube", ref cubePose, cube.Bounds);
			cube.Draw(cubePose.ToMatrix());

			// UI Window
            UI.WindowBegin("Window", ref windowPose, new Vec2(20, 0) * U.cm, showHeader ? UIWin.Normal : UIWin.Body);
            UI.Toggle("Show Header", ref showHeader);

            // Slider example
            UI.Label("Slide");
            UI.SameLine();
            UI.HSlider("slider", ref slider, -0.01f, .01f, .001f, 72 * U.mm);

			// Cube roaming button
			UI.Toggle("Cube roaming", ref moveCube);

			//if (UI.ButtonRound("Exit", null))
			//    SK.Quit();

			UI.WindowEnd();
        }
    }
}
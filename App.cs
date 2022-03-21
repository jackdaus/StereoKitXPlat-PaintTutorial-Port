using StereoKit;
using System.Numerics;

namespace StereoKitApp
{
	public class App
	{
		public SKSettings Settings => new SKSettings { 
			appName           = "StereoKit Ink",
			assetsFolder      = "Assets",
			displayPreference = DisplayMode.MixedReality
		};

		Matrix4x4 floorTransform = Matrix.TS(new Vector3(0, -1.5f, 0), new Vector3(30, 0.1f, 30));
		Material  floorMaterial;

		// For SteroKitInk
		static Painting activePainting = new Painting();
		static PaletteMenu palleteMenu;
		static Pose menuPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(-1, 0, 1));
		static Sprite appLogo;

		public void Init()
		{
			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			palleteMenu = new PaletteMenu();

			appLogo = Sprite.FromFile("StereoKitInkLight.png");
		}

		public void Step()
		{
            if (SK.System.displayType == Display.Opaque)
                Default.MeshCube.Draw(floorMaterial, floorTransform);

            activePainting.Step(Handed.Right, palleteMenu.PaintColor, palleteMenu.PaintSize);
            palleteMenu.Step();
            StepMenuWindow();
        }

		static void StepMenuWindow()
		{
			// Begin the application's menu window, we'll draw this without a
			// head bar (Body only) since we have a nice application image we can
			// add instead!
			UI.WindowBegin("Menu", ref menuPose, UIWin.Body);

			// Just draw the application logo across the top of the Menu window!
			// Vec2.Zero here tells StereoKit to auto-size both axes, so this
			// will automatically expand to the width of the window.
			UI.Image(appLogo, V.XY(UI.LayoutRemaining.x, 0));

			// Add undo and redo to the main menu, these are both available on
			// the radial menu, but these are easier to discover, and it never
			// hurts to have multiple options!
			if (UI.Button("Undo")) activePainting?.Undo();
			UI.SameLine();
			if (UI.Button("Redo")) activePainting?.Redo();

			// When the user presses the save button, lets show a save file
			// dialog! When a file name and folder have been selected, it'll make
			// a call to SavePainting with the file's path name with the .skp
			// extension.
			if (UI.Button("Save"))
				Platform.FilePicker(PickerMode.Save, SavePainting, null, ".skp");

			// And on that same line, we'll have a load button! This'll let the
			// user pick out any .skp files, and will call LoadPainting with the
			// selected file.
			UI.SameLine();
			if (UI.Button("Load"))
				Platform.FilePicker(PickerMode.Open, LoadPainting, null, ".skp");

			// Some visual separation
			UI.HSeparator();

			// Clear is easy! Just create a new Painting object!
			if (UI.Button("Clear"))
				activePainting = new Painting();

			// And if they want to quit? Just tell StereoKit! This will let
			// StereoKit finish the the frame properly, and then break out of the
			// Step loop above.
			UI.SameLine();
			if (UI.Button("Quit"))
				SK.Quit();

			// And end the window!
			UI.WindowEnd();
		}

		static void LoadPainting(string file)
			=> activePainting = Painting.FromFile(Platform.ReadFileText(file) ?? "");

		static void SavePainting(string file)
			=> Platform.WriteFile(file, activePainting.ToFileData());
	}
}
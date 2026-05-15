using Raylib_cs;
using System.Dynamic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks.Sources;
using System.Xml.Serialization;

namespace _2Drender3D;

class Program
{
    const int windowWidth = 1000;
    const int windowHeight = 800;
    const string windowTitle = "2DRender3D";
    const double camera_scale = 500;
    const double move_scale = 3;
    const double mouse_flex = 200;
    public static Vector3 camera_pos = new Vector3(0, 0, 0);
    public static Vector3 vRight = new Vector3(1, 0, 0);
    public static Vector3 vUp = new Vector3(0, 1, 0);
    public static Vector3 vFront = new Vector3(0, 0, 0);
    public static Vector2 center_pos = new Vector2(windowWidth / 2, windowHeight / 2);
    public static Vector3 vY_axis = new Vector3(0, 1, 0);
    public static Vector3 vZ_axis = new Vector3(0, 0, 1);

    const int FPS = 60;

    public static List<Vector3> cube_vecs = new List<Vector3>();
    public static void Main()
    {
        cube_vecs.Add(new Vector3(-100, -100, 200));
        cube_vecs.Add(new Vector3(-100, 100, 200));
        cube_vecs.Add(new Vector3(100, -100, 200));
        cube_vecs.Add(new Vector3(100, 100, 200));
        cube_vecs.Add(new Vector3(-100, -100, 400));
        cube_vecs.Add(new Vector3(-100, 100, 400));
        cube_vecs.Add(new Vector3(100, -100, 400));
        cube_vecs.Add(new Vector3(100, 100, 400));

        Raylib.InitWindow(windowWidth, windowHeight, windowTitle);
        Raylib.SetTargetFPS(FPS);
        Raylib.DisableCursor();
        Vector2 mouse_pos = Raylib.GetMousePosition();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            mouse_pos = Raylib.GetMousePosition();
            Raylib.SetMousePosition(Convert.ToInt32(center_pos.X), Convert.ToInt32(center_pos.Y));
            Vector2 delta_pos = mouse_pos - center_pos;
            double dtheta = delta_pos.X / mouse_flex;
            double dphi = delta_pos.Y / mouse_flex;
            Quaternion q_y_axis = Quaternion.CreateFromAxisAngle(vY_axis, Convert.ToSingle(dtheta));
            Vector3 new_vRight = Vector3.Transform(vRight, q_y_axis);
            Vector3 new_vUp = Vector3.Transform(vUp, q_y_axis);
            vRight = new_vRight;
            vUp = new_vUp;
            Quaternion q_vRight = Quaternion.CreateFromAxisAngle(vRight, Convert.ToSingle(dphi));
            Vector3 new_vUp2 = Vector3.Transform(vUp, q_vRight);
            vUp = new_vUp2;

            vRight = Vector3.Normalize(vRight);
            vUp = Vector3.Normalize(vUp);
            vFront = Vector3.Normalize(Vector3.Cross(vRight, vUp));

            Vector3 vForward = Vector3.Normalize(new Vector3(vFront.X, 0, vFront.Z));

            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                camera_pos += vForward;
            }
            if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                camera_pos -= vForward;
            }
            if (Raylib.IsKeyDown(KeyboardKey.A))
            {
                camera_pos -= vRight;
            }
            if (Raylib.IsKeyDown(KeyboardKey.D))
            {
                camera_pos += vRight;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Space))
            {
                camera_pos += new Vector3(0, Convert.ToSingle(move_scale), 0);
            }
            if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
            {
                camera_pos -= new Vector3(0, Convert.ToSingle(move_scale), 0);
            }

            DrawLine(0, 1);
            DrawLine(0, 2);
            DrawLine(0, 4);
            DrawLine(1, 3);
            DrawLine(1, 5);
            DrawLine(2, 3);
            DrawLine(2, 6);
            DrawLine(3, 7);
            DrawLine(4, 5);
            DrawLine(4, 6);
            DrawLine(5, 7);
            DrawLine(6, 7);

            Raylib.EndDrawing();
        }
    }

    public static Vector2 ProjectToCamera(Vector3 original_vector)
    {
        double distance = Vector3.Dot(vFront, original_vector - camera_pos);
        Vector3 project_vector = original_vector - Convert.ToSingle(distance) * vFront;
        Vector3 project_face_vector = project_vector - camera_pos;
        if(distance <= 1e-5)
        {
            return new Vector2(0xdeadbeef, 0xdeadbeef);
        }
        float x = Vector3.Dot(project_face_vector, vRight) / Convert.ToSingle(distance) * Convert.ToSingle(camera_scale);
        float y = Vector3.Dot(project_face_vector, vUp) / Convert.ToSingle(distance) * Convert.ToSingle(camera_scale);
        Vector2 vRes = new Vector2(x, y);
        return vRes;
    }

    public static Vector2 ConvertVector2ToPos(Vector2 pos)
    {
        float x = pos.X + center_pos.X;
        float y = center_pos.Y - pos.Y;
        return new Vector2(x, y);
    }

    public static void DrawLine(int i, int j)
    {
        Vector3 p1 = cube_vecs[i];
        Vector3 p2 = cube_vecs[j];

        double d1 = Vector3.Dot(vFront, p1 - camera_pos);
        double d2 = Vector3.Dot(vFront, p2 - camera_pos);

        if (d1 <= 0 && d2 <= 0) return;
        if (d1 <= 0 || d2 <= 0) 
        {
            double k = d1 / (d1 - d2);
            Vector3 pAmong = p1 + Convert.ToSingle(k) * (p2 - p1);
            pAmong += vFront * 0.01f;
            if (d1 <= 0) p1 = pAmong;
            else p2 = pAmong;
        }

        Vector2 vec1 = ProjectToCamera(p1);
        Vector2 vec2 = ProjectToCamera(p2);

        Vector2 pos1 = ConvertVector2ToPos(vec1);
        Vector2 pos2 = ConvertVector2ToPos(vec2);
        Raylib.DrawLineEx(pos1, pos2, 5f, Color.Black);
    }
}
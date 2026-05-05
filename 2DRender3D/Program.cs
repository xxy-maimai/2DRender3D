using Raylib_cs;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks.Sources;

namespace _2Drender3D;

class Program
{
    const int windowWidth = 1000;
    const int windowHeight = 800;
    const string windowTitle = "2DRender3D";
    const double camera_scale = 200;
    const double move_scale = 3;
    public static _3Dvector camera_pos = new _3Dvector(0, 0, 0);
    public static _3Dvector e1 = new _3Dvector(1, 0, 0), e2 = new _3Dvector(0, 0, 1);
    public static _3Dvector e3 = new _3Dvector(0, 1, 0);

    const int FPS = 60;

    public static List<_3Dvector> cube_vecs = new List<_3Dvector>();
    public static void Main()
    {
        cube_vecs.Add(new _3Dvector(-100, 200, -100));
        cube_vecs.Add(new _3Dvector(-100, 200, 100));
        cube_vecs.Add(new _3Dvector(100, 200, -100));
        cube_vecs.Add(new _3Dvector(100, 200, 100));
        cube_vecs.Add(new _3Dvector(-100, 400,- 100));
        cube_vecs.Add(new _3Dvector(-100, 400, 100));
        cube_vecs.Add(new _3Dvector(100, 400, -100));
        cube_vecs.Add(new _3Dvector(100, 400, 100));

        Raylib.InitWindow(windowWidth, windowHeight, windowTitle);
        Raylib.SetTargetFPS(FPS);
        Raylib.DisableCursor();
        Vector2 center_mouse_pos = new Vector2(windowWidth / 2, windowHeight / 2);
        Vector2 new_mouse_pos = new Vector2();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            new_mouse_pos = Raylib.GetMousePosition();
            Raylib.SetMousePosition(windowWidth / 2, windowHeight / 2);
            Vector2 delta_pos = center_mouse_pos - new_mouse_pos;
            double dtheta = delta_pos.X / 200;
            double dphi = delta_pos.Y / 200;
            e3 = _3Dvector.cross(e2, e1);

            _4Number z_number = new _4Number(Math.Cos(dtheta / 2), 0, 0, Math.Sin(dtheta / 2));
            _4Number z_number_ = new _4Number(Math.Cos(dtheta / 2), 0, 0, -Math.Sin(dtheta / 2));
            _4Number e1_number = new _4Number(0, e1.x, e1.y, e1.z);
            _4Number e2_number = new _4Number(0, e2.x, e2.y, e2.z);
            e1_number = z_number * e1_number * z_number_;
            e2_number = z_number * e2_number * z_number_;
            _4Number e1_number_rotate = new _4Number(Math.Cos(dphi / 2), Math.Sin(dphi / 2) * e1_number.i, Math.Sin(dphi / 2) * e1_number.j, Math.Sin(dphi / 2) * e1_number.k);
            _4Number e1_number_rotate_ = new _4Number(Math.Cos(dphi / 2), -Math.Sin(dphi / 2) * e1_number.i, -Math.Sin(dphi / 2) * e1_number.j, -Math.Sin(dphi / 2) * e1_number.k);
            e2_number = e1_number_rotate * e2_number * e1_number_rotate_;
            e1.x = e1_number.i;
            e1.y = e1_number.j;
            e1.z = e1_number.k;
            e2.x = e2_number.i;
            e2.y = e2_number.j;
            e2.z = e2_number.k;


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

            if (Raylib.IsKeyDown(KeyboardKey.D))
            {
                camera_pos += move_scale * e1;
            }
            if (Raylib.IsKeyDown(KeyboardKey.A))
            {
                camera_pos -= move_scale * e1;
            }
            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                camera_pos += move_scale * e3;
            }
            if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                camera_pos -= move_scale * e3;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Space))
            {
                camera_pos += move_scale * e2;
            }
            if(Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift)){
                camera_pos -= move_scale * e2;
            }

            Raylib.ClearBackground(Color.White);
            Raylib.EndDrawing();
        }
    }

    public static _2Dvector Convert3DVTo2DV(_3Dvector v3)
    {
        
        _3Dvector a = new _3Dvector(0, 0, 0);
        _3Dvector b = new _3Dvector(0, 0, 0);
        /*e3.x = -e3.x;
        e3.y = -e3.y;
        e3.z = -e3.z;*/
        //Console.WriteLine($"{e3.y}");
        a = v3 - (e3 * (v3 - camera_pos) / (e3 * e3)) * e3;
        b = a - camera_pos;
        double d = e3 * (v3 - camera_pos) / (e3 * e3);
        if(d < 0)
        {
            return new _2Dvector(0xdeadbeef, 0xdeadbeef);
        }
        double x, y;
        x = b * e1 / (e1 * e1) / d * camera_scale;
        y = b * e2 / (e2 * e2) / d * camera_scale;

        _2Dvector resv = new _2Dvector(x, y);
        return resv;
    }

    public static int Convert2DVToPosx(_2Dvector v)
    {
        int x = Convert.ToInt32(v.x) + windowWidth / 2;
        return x;
    }

    public static int Convert2DVToPosy(_2Dvector v)
    {
        int y = windowHeight / 2 - Convert.ToInt32(v.y);
        return y;
    }

    public static void DrawLine(int i, int j)
    {
        _2Dvector vec1 = Convert3DVTo2DV(cube_vecs[i]);
        _2Dvector vec2 = Convert3DVTo2DV(cube_vecs[j]);

        if(vec1.x == 0xdeadbeef &&  vec1.y == 0xdeadbeef)
        {
            return;
        }
        if (vec2.x == 0xdeadbeef && vec2.y == 0xdeadbeef)
        {
            return;
        }

        int x1 = Convert2DVToPosx(vec1);
        int y1 = Convert2DVToPosy(vec1);
        int x2 = Convert2DVToPosx(vec2);
        int y2 = Convert2DVToPosy(vec2);
        Vector2 v1 = new Vector2(x1, y1);
        Vector2 v2 = new Vector2(x2, y2);
        Raylib.DrawLineEx(v1, v2, 5f, Color.Black);
    }
}

class _3Dvector
{
    public double x, y, z;
    public _3Dvector(double inx,  double iny, double inz)
    {
        x = inx;
        y = iny;
        z = inz;
    }

    public static _3Dvector cross(_3Dvector a, _3Dvector b)
    {
        _3Dvector res = new _3Dvector(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        return res;
    }

    public static _3Dvector operator + ( _3Dvector a, _3Dvector b)
    {
        _3Dvector res = new _3Dvector(0, 0, 0);
        res.x = a.x;
        res.y = a.y;
        res.z = a.z;
        res.x += b.x;
        res.y += b.y;
        res.z += b.z;
        return res;
    }

    public static _3Dvector operator - (_3Dvector a, _3Dvector b)
    {
        _3Dvector res = new _3Dvector(0, 0, 0);
        res.x = a.x;
        res.y = a.y;
        res.z = a.z;
        res.x -= b.x;
        res.y -= b.y;
        res.z -= b.z;
        return res;
    }

    public static double operator * (_3Dvector a, _3Dvector b)
    {
        double res = 0;
        res += a.x * b.x;
        res += a.y * b.y;
        res += a.z * b.z;
        return res;
    }

    public static _3Dvector operator * (double a, _3Dvector b)
    {
        return new _3Dvector(b.x * a, b.y * a, b.z * a);
    }
}

class _2Dvector
{
    public double x, y;

    public _2Dvector(double inx, double iny)
    {
        x = inx;
        y = iny;
    }
}

class _4Number
{
    public double r, i, j, k;
    public _4Number(double r, double i, double j, double k)
    {
        this.r = r;
        this.i = i;
        this.j = j;
        this.k = k;
    }

    public static _4Number operator + (_4Number a, _4Number b)
    {
        _4Number res = new _4Number(0, 0, 0, 0);
        res.r = a.r;
        res.i = a.i;
        res.j = a.j;
        res.k = a.k;
        res.r += b.r;
        res.i += b.i;
        res.j += b.j;
        res.k += b.k;
        return res;
    }

    public static _4Number operator -(_4Number a, _4Number b)
    {
        _4Number res = new _4Number(0, 0, 0, 0);
        res.r = a.r;
        res.i = a.i;
        res.j = a.j;
        res.k = a.k;
        res.r -= b.r;
        res.i -= b.i;
        res.j -= b.j;
        res.k -= b.k;
        return res;
    }

    public static _4Number operator *(_4Number a, _4Number b)
    {
        _4Number res = new _4Number(0, 0, 0, 0);
        res.r = a.r * b.r - a.i * b.i - a.j * b.j - a.k * b.k;
        res.i = a.r * b.i + a.i * b.r - a.k * b.j + a.j * b.k;
        res.j = a.j * b.r + a.r * b.j - a.i * b.k + a.k * b.i;
        res.k = a.r * b.k + a.k * b.r - a.j * b.i + a.i * b.j;
        return res;
    }
}
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace threeD;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DispatcherTimer timer = new();




    // 声明场景对象
    Viewport3D viewPort = new();

    /*
        Model3DGroup 是一个容器，可以包含多个 Model3D 对象
        例如 GeometryModel3D、Light 等
        通过将几何模型添加到模型组中，你可以对一组模型进行统一的操作
        例如变换（平移、旋转、缩放）或应用同一组灯光
     */
    Model3DGroup modelGroup = new();

    /*
    提供所有可视对象共有的服务和属性，包括点击测试、坐标转换和边界框计算

    ModelVisual3D类有一个Children属性，可以用它来构建ModelVisual3D对象的树形结构
    ModelVisual3D对象被优化为场景节点。
    例如，它们缓存边界。只要可以，就使用ModelVisual3D对象来创建场景中对象的独特实例

    这种用法与Model3D对象的用法相反，Model3D对象是轻量级对象，经过优化后可以共享和重用。
    例如，使用Model3D对象构建汽车模型;并使用10个ModelVisual3D对象在你的场景中放置10辆汽车。
    */
    ModelVisual3D modelVisual = new();



    AxisAngleRotation3D rotation = new();
    RotateTransform3D transformer = new();


    public MainWindow()
    {
        InitializeComponent();

        Create3DView();

        InitializeTimer();

        // 订阅CompositionTarget.Rendering事件
        CompositionTarget.Rendering += CompositionTarget_Rendering;
    }

    private void InitializeTimer()
    {
        timer.Interval = TimeSpan.FromMilliseconds(100);// 设置时间间隔
        timer.Tick += timerTick; // 订阅Tick事件
        //timer.Start(); // 启动定时器
    }

    private void timerTick(object? sender, EventArgs e)
    {
        // 每次 Tick 事件触发，增加旋转角度
        rotation.Angle += 1; // 递增角度，例如每次增加1度

        // 应用旋转变换
        transformer.Rotation = rotation;
    }

    private void CompositionTarget_Rendering(object? sender, EventArgs e)
    {
        // 每次 Tick 事件触发，增加旋转角度
        rotation.Angle += 1; // 递增角度，例如每次增加1度

        // 应用旋转变换
        transformer.Rotation = rotation;
    }

    void Create3DView()
    {
        // 将摄像机指定到视口
        viewPort.Camera = CreateCamera();

        // 添加照明灯光
        modelGroup.Children.Add(CreateLight());

        // 将几何模型添加到模型组
        modelGroup.Children.Add(CreateModel());

        // 将模型组添加到 ModelVisual3d 中
        modelVisual.Content = modelGroup;

        viewPort.Children.Add(modelVisual);

        // 将视口应用到页面，以便渲染
        this.Content = viewPort;
    }

    private System.Windows.Media.Media3D.Geometry3D CreateMesh()
    {
        // 用于生成三维形状的三角形基元, 这不是3维形状
        MeshGeometry3D mesh = new();

        // 为 MeshGeometry3D 创建法线矢量集合
        /*
        这是顶点法线向量
        法线确定给定的三角形面是否亮起
        通常每个面的法线向量是计算该面的顶点法线的平均值

        如果未指定法线，则其生成取决于开发人员是否为网格指定了三角形索引
            如果指定了三角形索引，则将生成考虑相邻面的法线
            如果未指定三角形索引，则只会为指定的三角形生成一个法线,这可能会导致网格中出现分面外观
         */
        Vector3DCollection myNormalCollection = new()
        {
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),
        };
        // 可以不指定由软件 计算 网格三角形索引 得到
        //model.Normals = myNormalCollection;


        // 为 MeshGeometry3D 创建顶点位置集合
        Point3DCollection myPositionCollection = new()
        {
            new Point3D(-0.5, -0.5, 0.5),
            new Point3D(0.5, -0.5, 0.5),
            new Point3D(0.5, 0.5, 0.5),
            new Point3D(0.5, 0.5, 0.5),
            new Point3D(-0.5, 0.5, 0.5),
            new Point3D(-0.5, -0.5, 0.5),
        };
        mesh.Positions = myPositionCollection;

        // 为网格几何体 3D 创建纹理坐标集合
        // TextureCoordinates用于描述材质如何贴在平面上。材质是用一个矩形去描述的
        PointCollection myTextureCoordinatesCollection = new()
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 1),
            new Point(0, 1),
            new Point(0, 0),
        };
        mesh.TextureCoordinates = myTextureCoordinatesCollection;

        // 为网格几何体 3D 创建三角形索引集合
        // 哪些点构成一个三角行(3个点一组)
        Int32Collection myTriangleIndicesCollection = new()
        {
            0,
            1,
            2,
            3,
            4,
            5
        };
        mesh.TriangleIndices = myTriangleIndicesCollection;

        return mesh;
    }

    private System.Windows.Media.Media3D.Geometry3D CreateMeshCube()
    {
        MeshGeometry3D mesh = new();

        // 为 MeshGeometry3D 创建顶点位置集合
        Point3DCollection myPositionCollection = new()
        {
            // 顶面顶点
            new Point3D(0.5, 0.5, 0.5),  // 0
            new Point3D(0.5, 0.5, -0.5), // 1
            new Point3D(-0.5, 0.5, -0.5),// 2
            new Point3D(-0.5, 0.5, 0.5), // 3

            // 底面顶点
            new Point3D(0.5, -0.5, 0.5), // 4
            new Point3D(0.5, -0.5, -0.5),// 5
            new Point3D(-0.5, -0.5, -0.5),// 6
            new Point3D(-0.5, -0.5, 0.5), // 7
        };
        mesh.Positions = myPositionCollection;

        // 为网格几何体 3D 创建三角形索引集合
        Int32Collection myTriangleIndicesCollection = new()
        {
            // 顶面
            0, 1, 2,
            2, 3, 0,

            // 底面
            4, 6, 5,
            6, 4, 7,

            // 前面
            0, 3, 7,
            7, 4, 0,

            // 后面
            1, 5, 6,
            6, 2, 1,

            // 左面
            3, 2, 6,
            6, 7, 3,

            // 右面
            0, 4, 5,
            5, 1, 0,
        };
        mesh.TriangleIndices = myTriangleIndicesCollection;

        // 为网格几何体 3D 创建法线矢量集合
        Vector3DCollection myNormalCollection = new()
        {
            // 顶面法线
            new Vector3D(0, 1, 0),
            new Vector3D(0, 1, 0),
            new Vector3D(0, 1, 0),
            new Vector3D(0, 1, 0),

            // 底面法线
            new Vector3D(0, -1, 0),
            new Vector3D(0, -1, 0),
            new Vector3D(0, -1, 0),
            new Vector3D(0, -1, 0),

            // 前面法线
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, 1),

            // 后面法线
            new Vector3D(0, 0, -1),
            new Vector3D(0, 0, -1),
            new Vector3D(0, 0, -1),
            new Vector3D(0, 0, -1),

            // 左面法线
            new Vector3D(-1, 0, 0),
            new Vector3D(-1, 0, 0),
            new Vector3D(-1, 0, 0),
            new Vector3D(-1, 0, 0),

            // 右面法线
            new Vector3D(1, 0, 0),
            new Vector3D(1, 0, 0),
            new Vector3D(1, 0, 0),
            new Vector3D(1, 0, 0),
        };
        //mesh.Normals = myNormalCollection;

        // 为网格几何体 3D 创建纹理坐标集合
        // 每个顶点的纹理坐标
        PointCollection myTextureCoordinatesCollection = new()
        {
            // 顶面
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),

            // 底面
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),

            // 前面
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),

            // 后面
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),

            // 左面
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),

            // 右面
            new Point(0, 0),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),
        };
        mesh.TextureCoordinates = myTextureCoordinatesCollection;

        return mesh;
    }


    private System.Windows.Media.Media3D.Camera CreateCamera()
    {
        // 定义用于查看 3D 物体的摄像机
        // 要查看 3D 物体，摄像机的位置和指向必须使物体在摄像机的视线范围内
        PerspectiveCamera pCamera = new()
        {
            // 指定摄像机在 3D 场景中的位置
            Position = new Point3D(0, 0, 2),
            // 指定摄像机指向的方向
            LookDirection = new Vector3D(0, 0, -1),
            // 以度为单位定义摄像机的水平视场
            FieldOfView = 60,
        };

        return pCamera;
    }


    private System.Windows.Media.Media3D.Light CreateLight()
    {
        // 表示应用到三维场景的照明的 Model3D 对象

        // 定义场景中投射的光线。
        // 没有灯光，就无法看到 3D 物体。
        // 注意：要从其他方向照亮物体，请创建其他灯光。
        DirectionalLight light = new()
        {
            Color = Colors.White,
            Direction = new Vector3D(-0.61, -0.5, -0.61),
        };
        return light;
    }

    private System.Windows.Media.Media3D.Material CreateMaterial()
    {
        // 材质指定应用于 3D 物体的材质。在本示例中，线性渐变覆盖了 3D 物体的表面。

        // 创建四档水平线性渐变
        LinearGradientBrush gradient = new()
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
        };
        gradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
        gradient.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
        gradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
        gradient.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));

        // 定义材质并应用于网格几何图形
        DiffuseMaterial material = new(gradient);

        return material;
    }

    private System.Windows.Media.Media3D.Transform3D ModelTransForm()
    {
        // 对对象进行变换
        rotation.Axis = new Vector3D(1, 3, 0);
        rotation.Angle = 40;

        transformer.Rotation = rotation;

        return transformer;
    }

    private System.Windows.Media.Media3D.Model3D CreateModel()
    {
        GeometryModel3D model = new();

        // 将网格应用于几何模型
        //model.Geometry = CreateMesh();
        model.Geometry = CreateMeshCube();

        // 应用材质(表面)
        model.Material = CreateMaterial();

        // 对对象进行变换
        model.Transform = ModelTransForm();

        return model;
    }

}
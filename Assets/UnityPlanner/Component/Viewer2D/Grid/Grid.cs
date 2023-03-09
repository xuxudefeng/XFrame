using UnityEngine;
using UnityEngine.UIElements;

public class Grid : VisualElement
{
    // 网格参数
    public int gridSpacing = 20;
    public float gridWidth = 1;
    public float gridHeight = 1;
    public float lineWidth = 1f;
    public string GridColor = "#E0E0E0";////"#f1f1f1";////"#D0D0D0"
    public Color gridColor;

    public Color heightColor;

    public string backgroundCorlor = "#f1f1f1";

    private void OnPointerMove(MouseMoveEvent evt)
    {
        Debug.Log(evt.mousePosition);
    }

    public Grid(float width, float height)
    {
        gridWidth = width;
        gridHeight = height;


        this.style.width = width;
        this.style.height = height;
        this.style.backgroundColor = Color.white;

        ColorUtility.TryParseHtmlString(GridColor, out gridColor);

        ColorUtility.TryParseHtmlString("#D0D0D0", out heightColor);

        this.RegisterCallback<MouseMoveEvent>(OnPointerMove);

        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        this.drawGrid(mgc.painter2D);
    }

    private void drawLine(Painter2D painter2D, Vector2 start, Vector2 end, float lineWidth, Color strokeColor)
    {
        painter2D.lineWidth = lineWidth;
        painter2D.strokeColor = strokeColor;
        painter2D.lineJoin = LineJoin.Round;

        painter2D.BeginPath();
        painter2D.MoveTo(start);
        painter2D.LineTo(end);
        painter2D.Stroke();
    }

    private float calculateGridOffset(int n)
    {
        if (n >= 0)
        {
            return (n + this.gridSpacing / 2.0f) % this.gridSpacing - this.gridSpacing / 2.0f;
        }
        else
        {
            return (n - this.gridSpacing / 2.0f) % this.gridSpacing + this.gridSpacing / 2.0f;
        }
    }

    private void drawGrid(Painter2D painter2D)
    {
        var offsetX = this.calculateGridOffset(-0);// -this.viewmodel.originX);
        var offsetY = this.calculateGridOffset(-0);// -this.viewmodel.originY);
        // var width = gridWidth;
        // var height = gridHeight;
        for (var x = 0; x <= (this.gridWidth / gridSpacing); x++)
        {
            if (x % 10 == 0)
            {
                drawLine(painter2D, new Vector2(gridSpacing * x + offsetX, 0), new Vector2(gridSpacing * x + offsetX, this.gridHeight), lineWidth, heightColor);
            }
            else
            {
                drawLine(painter2D, new Vector2(gridSpacing * x + offsetX, 0), new Vector2(gridSpacing * x + offsetX, this.gridHeight), lineWidth, gridColor);
            }

        }
        for (var y = 0; y <= (this.gridHeight / gridSpacing); y++)
        {
            if (y % 10 == 0)
            {
                drawLine(painter2D, new Vector2(0, gridSpacing * y + offsetY), new Vector2(this.gridWidth, gridSpacing * y + offsetY), lineWidth, heightColor);
            }
            else
            {
                drawLine(painter2D, new Vector2(0, gridSpacing * y + offsetY), new Vector2(this.gridWidth, gridSpacing * y + offsetY), lineWidth, gridColor);
            }

        }
    }
}
// Drawing a line with UITK https://forum.unity.com/threads/drawing-a-line-with-uitk.1193470/
using UnityEngine;
using UnityEngine.UIElements;

public class Line : VisualElement
{
    public Vector2 startPos, endPos;
    public float LineWidth;
    public Color StrokeColor = Color.white;
    public LineJoin LineJoin = LineJoin.Round;
    public LineCap LineCap = LineCap.Round;

    public Line(Vector2 pos1, Vector2 pos2, float lineWidth)
    {
        startPos = pos1;
        endPos = pos2;
        LineWidth = lineWidth;

        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter2D = mgc.painter2D;
        painter2D.lineWidth = LineWidth;
        painter2D.strokeColor = StrokeColor;
        painter2D.lineJoin = LineJoin;
        painter2D.lineCap = LineCap;

        painter2D.BeginPath();
        painter2D.MoveTo(startPos);
        painter2D.LineTo(endPos);
        painter2D.Stroke();

    }
}
using OxyPlot;

namespace Spaghetti.Plot.Transforms;

public sealed class CartesianCoordinateTransformation : ICoordinateTransformation<DataPoint>
{
  public ICoordinateTransformation<double> TransformX { get; private set; }
  public ICoordinateTransformation<double> TransformY { get; private set; }

  public CartesianCoordinateTransformation(ICoordinateTransformation<double> transformX,
                                           ICoordinateTransformation<double> transformY)
  {
    TransformX = transformX;
    TransformY = transformY;
  }

  public DataPoint Forward(DataPoint value)
  {
    return new DataPoint(
      x: TransformX.Forward(value.X),
      y: TransformY.Forward(value.Y));
  }

  public DataPoint Backward(DataPoint value)
  {
    return new DataPoint(
      x: TransformX.Backward(value.X),
      y: TransformY.Backward(value.Y));
  }
}

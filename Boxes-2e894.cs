using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Mola;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_2e894 : GH_ScriptInstance
{
  #region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { /* Implementation hidden. */ }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { /* Implementation hidden. */ }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { /* Implementation hidden. */ }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
  #endregion

  #region Members
  /// <summary>Gets the current Rhino document.</summary>
  private readonly RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private readonly GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private readonly IGH_Component Component;
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private readonly int Iteration;
  #endregion
  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments,
  /// Output parameters as ref arguments. You don't have to assign output parameters,
  /// they will have a default value.
  /// </summary>
  #region Runscript
  private void RunScript(List<Point3d> x, List<Point3d> y, ref object A)
  {
    List<MolaMesh> meshes = new List<MolaMesh>();
    MolaMesh mesh = new MolaMesh();
    for (int i = 0; i < x.Count; i++)
    {
      float x1 = (float)x[i].X;
      float y1 = (float)x[i].Y;
      float z1 = (float)x[i].Z;
      float x2 = (float)y[i].X;
      float y2 = (float)y[i].Y;
      float z2 = (float)y[i].Z;

      mesh = MeshFactory.CreateBox(x1, y1, z1, x2, y2, z2);
      meshes.Add(mesh);
    }
    mesh = MeshTools.Merge(meshes);

    //mesh = MeshSubdivision.LinearSplitQuad(mesh, 2, 6);
    mesh = MeshSubdivision.Grid(mesh, 10, 10);
    mesh = MeshSubdivision.ExtrudeTapered(mesh, 10, 0.7f, true);
    mesh = MeshTools.WeldVertices(mesh);
    mesh = MeshTools.UpdateTopology(mesh);
    mesh = MeshSubdivision.CatmullClark(mesh);
    A = mesh;
  }
  #endregion
  #region Additional

  #endregion
}
using System.Collections.Generic;
using System;

namespace Mola
{
    public class MarchingSquare
    {
        public static MolaMesh MarchingSquares(float[][] field, Color[][] colors, float iso, float z = 0)
        {
            MolaMesh mesh = new MolaMesh();
            //Marching Squares isolines
            int cols = field.Length;
            int rows = field[0].Length;
            for (int i = 0; i < cols - 1; i++)
            {
                for (int j = 0; j < rows - 1; j++)
                {
                    float x = i;
                    float y = j;

                    int c1 = field[i][j] < iso ? 0 : 1;
                    int c2 = field[i + 1][j] < iso ? 0 : 1;
                    int c3 = field[i + 1][j + 1] < iso ? 0 : 1;
                    int c4 = field[i][j + 1] < iso ? 0 : 1;
                    int state = GetState(c1, c2, c3, c4);

                    //interpolation
                    /*The dot . is the field values
                     .___a___.
                     |       |
                     d       b
                     |       |
                     .___c___.
                     */
                    float a_val = field[i][j];
                    float b_val = field[i + 1][j];
                    float c_val = field[i + 1][j + 1];
                    float d_val = field[i][j + 1];

                    Color a_col = colors[i][j];
                    Color b_col = colors[i + 1][j];
                    Color c_col = colors[i + 1][j + 1];
                    Color d_col = colors[i][j + 1];

                    Vec3 a = new Vec3();
                    float amt = (iso - a_val) / (b_val - a_val);
                    a.x = Mathf.Lerp(x, x + 1, amt);
                    a.y = y;

                    Color ac = Color.Lerp(a_col, b_col, amt);

                    Vec3 b = new Vec3();
                    amt = (iso - b_val) / (c_val - b_val);
                    b.x = x + 1;
                    b.y = Mathf.Lerp(y, y + 1, amt);

                    Color bc = Color.Lerp(b_col, c_col, amt);


                    Vec3 c = new Vec3();
                    amt = (iso - d_val) / (c_val - d_val);
                    c.x = Mathf.Lerp(x, x + 1, amt);
                    c.y = y + 1;

                    Color cc = Color.Lerp(d_col, c_col, amt);


                    Vec3 d = new Vec3();
                    amt = (iso - a_val) / (d_val - a_val);
                    d.x = x;
                    d.y = Mathf.Lerp(y, y + 1, amt);

                    Color dc = Color.Lerp(a_col, c_col, amt);


                    //Marching squares rule table https://en.wikipedia.org/wiki/File:Marching_squares_algorithm.svg
                    if (state > 15)
                    {
                        state = -1;
                    }
                    switch (state)
                    {
                        case 1:
                            //bottom left triangle
                            mesh.AddTriangle(d.x, d.y, z, c.x, c.y, z, i, j + 1, z, dc, cc, a_col);
                            break;
                        case 2:
                            //bottom right triangle
                            //mesh.AddTri2D(b.x, b.y, i + 1, j + 1, c.x, c.y, z);
                            mesh.AddTriangle(b.x, b.y, z, i + 1, j + 1, z, c.x, c.y, z, bc, c_col, cc);

                            break;
                        case 3:
                            //retangular bottom half
                            // mesh.AddTri2D(d.x, d.y, b.x, b.y, i + 1, j + 1, z);
                            mesh.AddTriangle(d.x, d.y, z, b.x, b.y, z, i + 1, j + 1, z, dc, bc, c_col);

                            //mesh.AddTri2D(d.x, d.y, i + 1, j + 1, i, j + 1, z);
                            mesh.AddTriangle(d.x, d.y, z, i + 1, j + 1, z, i, j + 1, z, dc, c_col, d_col);

                            break;
                        case 4:
                            //top right triangle
                            //mesh.AddTri2D(a.x, a.y, i + 1, j, b.x, b.y, z);
                            mesh.AddTriangle(a.x, a.y, z, i + 1, j, z, b.x, b.y, z, ac, b_col, bc);

                            break;
                        case 5:
                            //bottom-left to top-right middle hexagon slice
                            //mesh.AddTri2D(a.x, a.y, i + 1, j, b.x, b.y, z);
                            mesh.AddTriangle(a.x, a.y, z, i + 1, j, z, b.x, b.y, z, ac, b_col, bc);
                            // mesh.AddTri2D(a.x, a.y, b.x, b.y, c.x, c.y, z);
                            mesh.AddTriangle(a.x, a.y, z, b.x, b.y, z, c.x, c.y, z, ac, bc, cc);
                            //mesh.AddTri2D(a.x, a.y, c.x, c.y, i, j + 1, z);
                            mesh.AddTriangle(a.x, a.y, z, c.x, c.y, z, i, j + 1, z, ac, cc, d_col);
                            //mesh.AddTri2D(a.x, a.y, i, j + 1, d.x, d.y, z);
                            mesh.AddTriangle(a.x, a.y, z, i, j + 1, z, d.x, d.y, z, ac, d_col, dc);
                            break;
                        case 6:
                            //retangular left side half
                            //mesh.AddTri2D(a.x, a.y, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTriangle(a.x, a.y, z, i + 1, j, z, i + 1, j + 1, z, ac, b_col, c_col);

                            //mesh.AddTri2D(a.x, a.y, i + 1, j + 1, c.x, c.y, z);
                            mesh.AddTriangle(a.x, a.y, z, i + 1, j + 1, z, c.x, c.y, z, ac, c_col, cc);

                            break;
                        case 7:
                            //Pentagonal shape from subtracted top-left triangle
                            //mesh.AddTri2D(a.x, a.y, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTriangle(a.x, a.y, z, i + 1, j, z, i + 1, j + 1, z, ac, b_col, c_col);

                            //mesh.AddTri2D(a.x, a.y, i + 1, j + 1, i, j + 1, z);
                            mesh.AddTriangle(a.x, a.y, z, i + 1, j + 1, z, i, j + 1, z, ac, c_col, d_col);

                            //mesh.AddTri2D(a.x, a.y, i, j + 1, d.x, d.y, z);
                            mesh.AddTriangle(a.x, a.y, z, i, j + 1, z, d.x, d.y, z, ac, d_col, dc);

                            break;
                        case 8:
                            //top-left triangle
                            //mesh.AddTri2D(i, j, a.x, a.y, d.x, d.y, z);
                            mesh.AddTriangle(i, j, z, a.x, a.y, z, d.x, d.y, z, a_col, ac, dc);

                            break;
                        case 9:
                            //left side rectangle
                            //mesh.AddTri2D(i, j, a.x, a.y, c.x, c.y, z);
                            mesh.AddTriangle(i, j, z, a.x, a.y, z, c.x, c.y, z, a_col, ac, cc);

                            //mesh.AddTri2D(i, j, c.x, c.y, i, j + 1, z);
                            mesh.AddTriangle(i, j, z, c.x, c.y, z, i, j + 1, z, a_col, cc, d_col);

                            break;
                        case 10:
                            //top-left to bottom-right middle hexagon slice
                            //mesh.AddTri2D(i, j, a.x, a.y, b.x, b.y, z);
                            mesh.AddTriangle(i, j, z, a.x, a.y, z, b.x, b.y, z, a_col, ac, bc);

                            // mesh.AddTri2D(i, j, b.x, b.y, i + 1, j + 1, z);
                            mesh.AddTriangle(i, j, z, b.x, b.y, z, i + 1, j + 1, z, a_col, bc, c_col);

                            //mesh.AddTri2D(i, j, i + 1, j + 1, c.x, c.y, z);
                            mesh.AddTriangle(i, j, z, i + 1, j + 1, z, c.x, c.y, z, a_col, c_col, cc);


                            //mesh.AddTri2D(i, j, c.x, c.y, d.x, d.y, z);
                            mesh.AddTriangle(i, j, z, c.x, c.y, z, d.x, d.y, z, a_col, cc, dc);

                            break;
                        case 11:
                            //Pentagonal shape from subtracted top-right triangle
                            //mesh.AddTri2D(i, j, a.x, a.y, b.x, b.y, z);
                            mesh.AddTriangle(i, j, z, a.x, a.y, z, b.x, b.y, z, a_col, ac, bc);
                            //mesh.AddTri2D(i, j, b.x, b.y, i + 1, j + 1, z);
                            mesh.AddTriangle(i, j, z, b.x, b.y, z, i + 1, j + 1, z, a_col, bc, b_col);

                            //mesh.AddTri2D(i, j, i + 1, j + 1, i, j + 1, z);
                            mesh.AddTriangle(i, j, z, i + 1, j + 1, z, i, j + 1, z, a_col, c_col, d_col);

                            break;
                        case 12:
                            //Top half rectangle
                            //mesh.AddTri2D(i, j, i + 1, j, b.x, b.y, z);
                            mesh.AddTriangle(i, j, z, i + 1, j, z, b.x, b.y, z, a_col, b_col, bc);

                            //mesh.AddTri2D(i, j, b.x, b.y, d.x, d.y, z);
                            mesh.AddTriangle(i, j, z, b.x, b.y, z, d.x, d.y, z, a_col, bc, dc);

                            break;
                        case 13:
                            //Pentagonal shape from subtracted bottom-right triangle
                            mesh.AddTri2D(i, j, i + 1, j, b.x, b.y, z);

                            mesh.AddTriangle(i, j, z, i + 1, j, z, b.x, b.y, z, a_col, b_col, bc);

                            // mesh.AddTri2D(i, j, b.x, b.y, c.x, c.y, z);
                            mesh.AddTriangle(i, j, z, b.x, b.y, z, c.x, c.y, z, a_col, bc, cc);

                            //mesh.AddTri2D(i, j, c.x, c.y, i, j + 1, z);
                            mesh.AddTriangle(i, j, z, c.x, c.y, z, i, j + 1, z, a_col, cc, d_col);

                            break;
                        case 14:
                            //Pentagonal shape from subtracted bottom-left triangle
                            //mesh.AddTri2D(i, j, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTriangle(i, j, z, i + 1, j, z, i + 1, j + 1, z, a_col, b_col, c_col);

                            //mesh.AddTri2D(i, j, i + 1, j + 1, c.x, c.y, z);
                            mesh.AddTriangle(i, j, z, i + 1, j + 1, z, c.x, c.y, z, a_col, c_col, cc);

                            //mesh.AddTri2D(i, j, c.x, c.y, d.x, d.y, z);
                            mesh.AddTriangle(i, j, z, c.x, c.y, z, d.x, d.y, z, a_col, cc, dc);

                            break;
                        case 15:
                            //Pentagonal shape from subtracted bottom-left triangle
                            //mesh.AddTri2D(i, j, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTriangle(i, j, z, i + 1, j, z, i + 1, j + 1, z, a_col, b_col, c_col);

                            //mesh.AddTri2D(i, j, i + 1, j + 1, i, j + 1, z);
                            mesh.AddTriangle(i, j, z, i + 1, j + 1, z, i, j + 1, z, a_col, c_col, d_col);

                            break;
                    }

                }
            }
            return mesh;
        }

        public static MolaMesh MarchingSquares(float[][] field, float iso, float z = 0)
        {
            MolaMesh mesh = new MolaMesh();
            //Marching Squares isolines
            int cols = field.Length;
            int rows = field[0].Length;
            for (int i = 0; i < cols - 1; i++)
            {
                for (int j = 0; j < rows - 1; j++)
                {
                    float x = i;
                    float y = j;

                    int c1 = field[i][j] < iso ? 0 : 1;
                    int c2 = field[i + 1][j] < iso ? 0 : 1;
                    int c3 = field[i + 1][j + 1] < iso ? 0 : 1;
                    int c4 = field[i][j + 1] < iso ? 0 : 1;
                    int state = GetState(c1, c2, c3, c4);

                    //interpolation
                    /*The dot . is the field values
                     .___a___.
                     |       |
                     d       b
                     |       |
                     .___c___.
                     */
                    float a_val = field[i][j];
                    float b_val = field[i + 1][j];
                    float c_val = field[i + 1][j + 1];
                    float d_val = field[i][j + 1];

                    Vec3 a = new Vec3();
                    float amt = (iso - a_val) / (b_val - a_val);
                    a.x = Mathf.Lerp(x, x + 1, amt);
                    a.y = y;

                    Vec3 b = new Vec3();
                    amt = (iso - b_val) / (c_val - b_val);
                    b.x = x + 1;
                    b.y = Mathf.Lerp(y, y + 1, amt);

                    Vec3 c = new Vec3();
                    amt = (iso - d_val) / (c_val - d_val);
                    c.x = Mathf.Lerp(x, x + 1, amt);
                    c.y = y + 1;

                    Vec3 d = new Vec3();
                    amt = (iso - a_val) / (d_val - a_val);
                    d.x = x;
                    d.y = Mathf.Lerp(y, y + 1, amt);

                    //Marching squares rule table https://en.wikipedia.org/wiki/File:Marching_squares_algorithm.svg
                    if (state > 15)
                    {
                        state = -1;
                    }
                    switch (state)
                    {
                        case 1:
                            //bottom left triangle
                            mesh.AddTri2D(d.x, d.y, c.x, c.y, i, j + 1, z);
                            break;
                        case 2:
                            //bottom right triangle
                            mesh.AddTri2D(b.x, b.y, i + 1, j + 1, c.x, c.y, z);
                            break;
                        case 3:
                            //retangular bottom half
                            mesh.AddTri2D(d.x, d.y, b.x, b.y, i + 1, j + 1, z);
                            mesh.AddTri2D(d.x, d.y, i + 1, j + 1, i, j + 1, z);
                            break;
                        case 4:
                            //top right triangle
                            mesh.AddTri2D(a.x, a.y, i + 1, j, b.x, b.y, z);
                            break;
                        case 5:
                            //bottom-left to top-right middle hexagon slice
                            mesh.AddTri2D(a.x, a.y, i + 1, j, b.x, b.y, z);
                            mesh.AddTri2D(a.x, a.y, b.x, b.y, c.x, c.y, z);
                            mesh.AddTri2D(a.x, a.y, c.x, c.y, i, j + 1, z);
                            mesh.AddTri2D(a.x, a.y, i, j + 1, d.x, d.y, z);
                            break;
                        case 6:
                            //retangular left side half
                            mesh.AddTri2D(a.x, a.y, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTri2D(a.x, a.y, i + 1, j + 1, c.x, c.y, z);
                            break;
                        case 7:
                            //Pentagonal shape from subtracted top-left triangle
                            mesh.AddTri2D(a.x, a.y, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTri2D(a.x, a.y, i + 1, j + 1, i, j + 1, z);
                            mesh.AddTri2D(a.x, a.y, i, j + 1, d.x, d.y, z);
                            break;
                        case 8:
                            //top-left triangle
                            mesh.AddTri2D(i, j, a.x, a.y, d.x, d.y, z);
                            break;
                        case 9:
                            //left side rectangle
                            mesh.AddTri2D(i, j, a.x, a.y, c.x, c.y, z);
                            mesh.AddTri2D(i, j, c.x, c.y, i, j + 1, z);
                            break;
                        case 10:
                            //top-left to bottom-right middle hexagon slice
                            mesh.AddTri2D(i, j, a.x, a.y, b.x, b.y, z);
                            mesh.AddTri2D(i, j, b.x, b.y, i + 1, j + 1, z);
                            mesh.AddTri2D(i, j, i + 1, j + 1, c.x, c.y, z);
                            mesh.AddTri2D(i, j, c.x, c.y, d.x, d.y, z);
                            break;
                        case 11:
                            //Pentagonal shape from subtracted top-right triangle
                            mesh.AddTri2D(i, j, a.x, a.y, b.x, b.y, z);
                            mesh.AddTri2D(i, j, b.x, b.y, i + 1, j + 1, z);
                            mesh.AddTri2D(i, j, i + 1, j + 1, i, j + 1, z);
                            break;
                        case 12:
                            //Top half rectangle
                            mesh.AddTri2D(i, j, i + 1, j, b.x, b.y, z);
                            mesh.AddTri2D(i, j, b.x, b.y, d.x, d.y, z);
                            break;
                        case 13:
                            //Pentagonal shape from subtracted bottom-right triangle
                            mesh.AddTri2D(i, j, i + 1, j, b.x, b.y, z);
                            mesh.AddTri2D(i, j, b.x, b.y, c.x, c.y, z);
                            mesh.AddTri2D(i, j, c.x, c.y, i, j + 1, z);
                            break;
                        case 14:
                            //Pentagonal shape from subtracted bottom-left triangle
                            mesh.AddTri2D(i, j, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTri2D(i, j, i + 1, j + 1, c.x, c.y, z);
                            mesh.AddTri2D(i, j, c.x, c.y, d.x, d.y, z);
                            break;
                        case 15:
                            //Pentagonal shape from subtracted bottom-left triangle
                            mesh.AddTri2D(i, j, i + 1, j, i + 1, j + 1, z);
                            mesh.AddTri2D(i, j, i + 1, j + 1, i, j + 1, z);
                            break;
                    }

                }
            }
            return mesh;
        }


        public static List<Vec3> MarchingContours(float[][] field, float iso)
        {
            MolaMesh mesh = new MolaMesh();
            //Marching Squares isolines
            int cols = field.Length;
            int rows = field[0].Length;
            List<Vec3> contours = new List<Vec3>();
            for (int i = 0; i < cols - 1; i++)
            {
                for (int j = 0; j < rows - 1; j++)
                {
                    float x = i;
                    float y = j;

                    int c1 = field[i][j] < iso ? 0 : 1;
                    int c2 = field[i + 1][j] < iso ? 0 : 1;
                    int c3 = field[i + 1][j + 1] < iso ? 0 : 1;
                    int c4 = field[i][j + 1] < iso ? 0 : 1;
                    int state = GetState(c1, c2, c3, c4);

                    //interpolation
                    /*The dot . is the field values
                     .___a___.
                     |       |
                     d       b
                     |       |
                     .___c___.
                     */
                    float a_val = field[i][j];
                    float b_val = field[i + 1][j];
                    float c_val = field[i + 1][j + 1];
                    float d_val = field[i][j + 1];

                    Vec3 a = new Vec3();
                    float amt = (iso - a_val) / (b_val - a_val);
                    a.x = Mathf.Lerp(x, x + 1, amt);
                    a.y = y;

                    Vec3 b = new Vec3();
                    amt = (iso - b_val) / (c_val - b_val);
                    b.x = x + 1;
                    b.y = Mathf.Lerp(y, y + 1, amt);

                    Vec3 c = new Vec3();
                    amt = (iso - d_val) / (c_val - d_val);
                    c.x = Mathf.Lerp(x, x + 1, amt);
                    c.y = y + 1;

                    Vec3 d = new Vec3();
                    amt = (iso - a_val) / (d_val - a_val);
                    d.x = x;
                    d.y = Mathf.Lerp(y, y + 1, amt);

                    //Marching squares rule table https://en.wikipedia.org/wiki/File:Marching_squares_algorithm.svg
                    if (state > 15)
                    {
                        state = -1;
                    }
                    switch (state)
                    {
                        case 1:
                            //bottom left triangle
                            contours.Add(d);
                            contours.Add(c);
                            break;
                        case 2:
                            //bottom right triangle
                            contours.Add(c);
                            contours.Add(b);
                            break;
                        case 3:
                            //retangular bottom half
                            contours.Add(d);
                            contours.Add(b);
                            break;
                        case 4:
                            //top right triangle
                            contours.Add(b);
                            contours.Add(a);
                            break;
                        case 5:
                            //bottom-left to top-right middle hexagon slice
                            contours.Add(d);
                            contours.Add(a);
                            contours.Add(b);
                            contours.Add(c);
                            break;
                        case 6:
                            //retangular left side half
                            contours.Add(c);
                            contours.Add(a);
                            break;
                        case 7:
                            //Pentagonal shape from subtracted top-left triangle
                            contours.Add(d);
                            contours.Add(a);
                            break;
                        case 8:
                            //top-left triangle
                            contours.Add(a);
                            contours.Add(d);
                            break;
                        case 9:
                            //left side rectangle
                            contours.Add(a);
                            contours.Add(c);
                            break;
                        case 10:
                            //top-left to bottom-right middle hexagon slice
                            contours.Add(a);
                            contours.Add(b);
                            contours.Add(c);
                            contours.Add(d);
                            break;
                        case 11:
                            //Pentagonal shape from subtracted top-right triangle
                            contours.Add(a);
                            contours.Add(b);
                            break;
                        case 12:
                            //Top half rectangle
                            contours.Add(b);
                            contours.Add(d);
                            break;
                        case 13:
                            //Pentagonal shape from subtracted bottom-right triangle
                            contours.Add(b);
                            contours.Add(c);
                            break;
                        case 14:
                            //Pentagonal shape from subtracted bottom-left triangle
                            contours.Add(c);
                            contours.Add(d);
                            break;
                        case 15:
                            break;
                    }

                }
            }
            return contours;
        }

        public static List<Vec3> MarchingContoursBorders(float[][] field, float iso)
        {
            MolaMesh mesh = new MolaMesh();
            //Marching Squares isolines
            int cols = field.Length;
            int rows = field[0].Length;
            List<Vec3> contours = new List<Vec3>();
            for (int i = 0; i < cols - 1; i++)
            {
                for (int j = 0; j < rows - 1; j++)
                {
                    if (i == 0 || i == cols - 2 || j == 0 || j == rows - 2)
                    {
                        float x = i;
                        float y = j;

                        int c1 = field[i][j] < iso ? 0 : 1;
                        int c2 = field[i + 1][j] < iso ? 0 : 1;
                        int c3 = field[i + 1][j + 1] < iso ? 0 : 1;
                        int c4 = field[i][j + 1] < iso ? 0 : 1;
                        int state = GetState(c1, c2, c3, c4);

                        //interpolation
                        /*The dot . is the field values
                         .___a___.
                         |       |
                         d       b
                         |       |
                         .___c___.
                         */
                        float a_val = field[i][j];
                        float b_val = field[i + 1][j];
                        float c_val = field[i + 1][j + 1];
                        float d_val = field[i][j + 1];
                        float[] values = new float[] { a_val, b_val, c_val, d_val };

                        bool[] border = new bool[4];

                        border[0] = j == 0;
                        border[1] = i == cols - 2;
                        border[2] = j == rows - 2;
                        border[3] = i == 0;


                        Vec3 ap = new Vec3(i, j);
                        Vec3 bp = new Vec3(i + 1, j);
                        Vec3 cp = new Vec3(i + 1, j + 1);
                        Vec3 dp = new Vec3(i, j + 1);
                        Vec3[] cell = new Vec3[] { ap, bp, cp, dp };

                        Vec3 a = new Vec3();
                        float amt = (iso - a_val) / (b_val - a_val);
                        a.x = Mathf.Lerp(x, x + 1, amt);
                        a.y = y;

                        Vec3 b = new Vec3();
                        amt = (iso - b_val) / (c_val - b_val);
                        b.x = x + 1;
                        b.y = Mathf.Lerp(y, y + 1, amt);

                        Vec3 c = new Vec3();
                        amt = (iso - d_val) / (c_val - d_val);
                        c.x = Mathf.Lerp(x, x + 1, amt);
                        c.y = y + 1;

                        Vec3 d = new Vec3();
                        amt = (iso - a_val) / (d_val - a_val);
                        d.x = x;
                        d.y = Mathf.Lerp(y, y + 1, amt);

                        Vec3[] intersections = new Vec3[] { a, b, c, d };

                        //Marching squares rule table https://en.wikipedia.org/wiki/File:Marching_squares_algorithm.svg
                        for (int index = 0; index < 4; index++)
                        {
                            int indexNext = index + 1;
                            if (indexNext == 4)
                            {
                                indexNext = 0;
                            }
                            if (border[index])
                            {
                                if (values[index] > iso && values[indexNext] > iso)
                                {
                                    contours.Add(cell[index]);
                                    contours.Add(cell[indexNext]);
                                }
                                if (values[index] > iso && values[indexNext] <= iso)
                                {
                                    contours.Add(cell[index]);
                                    contours.Add(intersections[index]);

                                }
                                if (values[index] <= iso && values[indexNext] > iso)
                                {
                                    contours.Add(intersections[index]);
                                    contours.Add(cell[indexNext]);
                                }
                            }
                        }

                    }
                }
            }
            return contours;
        }


        //Get the decimal value for the 4-bit binary number abcd
        public static int GetState(int a, int b, int c, int d)
        {
            return a * 8 + b * 4 + c * 2 + d * 1;
        }
    }

}
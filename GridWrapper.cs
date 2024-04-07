using System;
using System.Collections.Generic;
using System.Text;

namespace Mola
{
    /// <summary>
    /// this is a wrapper class to wrap MolaGrid from generic object to object
    /// </summary>
    /// ### Example
    /// ~~~~~~~~~~~~~~~~~~~~~~.cs
    /// MolaGrid<bool> grid = new MolaGrid<bool>(10, 10, 10);
    /// // wrap
    /// GridWrapper wrapper = new GridWrapper(grid);
    /// // unwrap
    /// MolaGrid<bool> unwrappedGrid = (MolaGrid<bool>)wrapper.gridObject;
    /// ~~~~~~~~~~~~~~~~~~~~~~
    public class GridWrapper
    {
        public object gridObject { get; private set; }
        public GridWrapper(object grid)
        {
            gridObject = grid;
        }
    }
}

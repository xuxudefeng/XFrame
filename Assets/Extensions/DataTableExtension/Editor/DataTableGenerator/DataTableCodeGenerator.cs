

// Copyright © 2013-2020 Jiang Yin. All rights reserved.




using System.Text;

namespace DE.Editor.DataTableTools
{
    public delegate void DataTableCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent,
        object userData);
}
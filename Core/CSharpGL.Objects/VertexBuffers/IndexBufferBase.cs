﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpGL.Objects.VertexBuffers
{
    /// <summary>
    /// 用于存储索引的VBO。
    /// </summary>
    public abstract class IndexBufferBase : VertexBuffer
    {
        /// <summary>
        /// 用于存储索引的VBO。
        /// </summary>
        /// <param name="mode">用哪种方式渲染各个顶点？（OpenGL.GL_TRIANGLES etc.）</param>
        /// <param name="usage"></param>
        public IndexBufferBase(DrawMode mode, BufferUsage usage)
            : base(usage)
        {
            this.Mode = mode;
        }

        /// <summary>
        /// 用哪种方式渲染各个顶点？（OpenGL.GL_TRIANGLES etc.）
        /// </summary>
        public DrawMode Mode { get; private set; }

    }


}

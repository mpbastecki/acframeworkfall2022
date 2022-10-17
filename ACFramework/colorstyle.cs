// For AC Framework 1.2, I've added some default parameters -- JC

using System;
using System.Drawing;

namespace ACFramework
{ 
	class cColorStyle 
	{ 
		public static readonly float LINEWIDTHWEIGHT = 0.05f; //Default linewidth 
		public const float LW_IGNORELINEWIDTHWEIGHT = 0.0f; /* Used as a linewidth value
			to mean ignore _linewidthweight and use _linewidth in graphicsMFC. */ 
		protected bool _filled; 
		protected bool _edged; 
		protected Color _fillcolor; 
		protected Color _linecolor; 
		protected float _linewidth; // Line width is the size at which I actually draw a line.
		protected float _linewidthweight; 
			/* In graphicsMFC, _linewidthweight is used to set _linewidth for	
			polygon to be a _linewidthweight * radius(). graphicsOpenGL just uses
			linewidth directly. */ 
		protected float _alpha; 
	//Construct and copy 
		
		public cColorStyle( )
        {
            _filled = true;
            _edged = true;
            _fillcolor = Color.LightGray;
            _linecolor = Color.Black;
            _linewidth = 1;
            _linewidthweight = cColorStyle.LW_IGNORELINEWIDTHWEIGHT;
            _alpha = 1.0f;
        }

        public cColorStyle( bool filled )
        {
            _filled = filled;
            _edged = true;
            _fillcolor = Color.LightGray;
            _linecolor = Color.Black;
            _linewidth = 1;
            _linewidthweight = cColorStyle.LW_IGNORELINEWIDTHWEIGHT;
            _alpha = 1.0f;
        }

        public cColorStyle( bool filled, bool edged )
        {
            _filled = filled;
            _edged = edged;
            _fillcolor = Color.LightGray;
            _linecolor = Color.Black;
            _linewidth = 1;
            _linewidthweight = cColorStyle.LW_IGNORELINEWIDTHWEIGHT;
            _alpha = 1.0f;
        }

        public cColorStyle( bool filled, bool edged, Color fillcolor )
        {
            _filled = filled;
            _edged = edged;
            _fillcolor = fillcolor;
            _linecolor = Color.Black;
            _linewidth = 1;
            _linewidthweight = cColorStyle.LW_IGNORELINEWIDTHWEIGHT;
            _alpha = 1.0f;
        }

        public cColorStyle( bool filled, bool edged, Color fillcolor, Color linecolor )
        {
            _filled = filled;
            _edged = edged;
            _fillcolor = fillcolor;
            _linecolor = linecolor;
            _linewidth = 1;
            _linewidthweight = cColorStyle.LW_IGNORELINEWIDTHWEIGHT;
            _alpha = 1.0f;
        }

        public cColorStyle(bool filled, bool edged, Color fillcolor, Color linecolor,
            float linewidth = 1.0f, 
            float linewidthweight = cColorStyle.LW_IGNORELINEWIDTHWEIGHT, float alpha = 1.0f )
        {
            _filled = filled;
            _edged = edged;
            _fillcolor = fillcolor;
            _linecolor = linecolor;
            _linewidth = linewidth;
            _linewidthweight = linewidthweight;
            _alpha = alpha;
        }
        
        /// <summary>
        /// Makes a copy of another cColorStyle object into this object.
        /// </summary>
        /// <param name="colorstyle">The cColorStyle object to make a copy of.</param>
        public virtual void copy(cColorStyle colorstyle) 
		{ 
			_filled = colorstyle._filled; 
			_edged = colorstyle._edged; 
			_fillcolor = colorstyle._fillcolor; 
			_linecolor = colorstyle._linecolor; 
			_linewidth = colorstyle._linewidth; 
			_linewidthweight = colorstyle._linewidthweight; 
			_alpha = colorstyle._alpha; 
		} 

		/// <summary>
		/// Mutates the cColorStyle object (randomly changes color, whether it is filled, and/or linewidth based on mutationflags)
		/// </summary>
		/// <param name="mutationflags">A collection of bits -- to enable color mutation, do a bit-wise or with cShape.MF_COLOR,
        /// to enable fill mutation, to a bit-wise or with cShape.MF_FILLING, and to enable linewidth mutation, do a 
        /// bit-wise or with cShape.MF_LINEWIDTH.</param>
		/// <param name="mutationstrength">The maximum random mutation (between 0 and 1)</param>
		public void mutate( int mutationflags, float mutationstrength ) 
		{ 
		//The colors 
			if ( (mutationflags & cShape.MF_COLOR) != 0 ) 
				_fillcolor = 
                    Color.FromArgb( Framework.randomOb.mutateColor( 
                        _fillcolor.ToArgb(), mutationstrength )); 
		//The filling 
			if ( (mutationflags & cShape.MF_FILLING) != 0 ) 
				_filled = _filled ^ Framework.randomOb.randomBOOL( mutationstrength / 2.0f ); 
				// Need to divide by two to get a proper coinflip; consider the case where mutationstrength is 1.0.
		//The linewidth 
			if ( (mutationflags & cShape.MF_LINEWIDTH) != 0 ) 
			{ 
				if ( Framework.randomOb.randomBOOL( 0.9f )) 
				{ 
					_linewidthweight = LW_IGNORELINEWIDTHWEIGHT; 
					_linewidth = 1.0f; 
				} 
				else 
				{ 
					_linewidthweight = 
							Framework.randomOb.mutate( _linewidthweight, 0.05f, 0.2f, mutationstrength ); 
					_linewidth = 
							Framework.randomOb.mutate( _linewidth, 1.0f, 7.0f, mutationstrength ); 
				} 
			} 
		} 

		
	//Accessor 
	
        /// <summary>
        /// Gets or sets _filled, whether or not the shape is filled
        /// </summary>
		public virtual bool Filled
		{
			get
				 { return _filled; }
			set
				{ _filled = value; }
		}

        /// <summary>
        /// Gets or set _edged, whether or not the shape has edges
        /// </summary>
		public virtual bool Edged
		{
			get
				 { return _edged; }
			set
				{ _edged = value; }
		}

        /// <summary>
        /// Gets or sets the _fillcolor of the shape
        /// </summary>
		public virtual Color FillColor
		{
			get
				 { return _fillcolor; }
			set
				 { _fillcolor = value; }
		}

        /// <summary>
        /// Gets or sets the _linecolor of the shape
        /// </summary>
		public virtual Color LineColor
		{
			get
				 { return _linecolor; }
			set
				{ _linecolor = value; }
		}

        /// <summary>
        /// Gets or sets the _linewidthweight of the shape
        /// </summary>
		public virtual float LineWidthWeight
		{
			get
				 { return _linewidthweight; }
			set
				{ _linewidthweight = value; }
		}

        /// <summary>
        /// Gets or sets the _linewidth of the shape
        /// </summary>
		public virtual float LineWidth
		{
			get
				 { return _linewidth; }
			set
				{ _linewidth = value; }
		}


	} 
}                                                                                                                                                                                                                                                                                                 
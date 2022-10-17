// In AC Framework Version 1.2, default _slices and _stacks is set to 32 instead of 16
// Default parameters were added -- JC

using System;

namespace ACFramework
{ 
	
	class cSpriteSphere : cSprite 
	{
        protected int _slices;
        protected int _stacks;
        protected cGLShape glshape;

        public cSpriteSphere( float radius )  
		{
            Radius = radius;
            _slices = (int) Radius * 10;
            _stacks = (int) Radius * 10;
            if (_slices < 32)
                _slices = 32;
            if (_stacks < 32)
                _stacks = 32;
            
            glshape = new cGLShape();
		} 

		//Default constructor calls initializer 
	//Overloaded cSprite methods 
	
		public void copy( cSpriteSphere pspritesphere ) 
        {
            base.copy(pspritesphere);
            _slices = pspritesphere._slices;
            _stacks = pspritesphere._stacks;
        }

        public override cSprite copy()
        {
            cSpriteSphere s = new cSphere( this.Radius, this.FillColor );
            s.copy(this);
            return s;
        }

		public override void mutate( int mutationflags, float mutationstrength ) 
		{ 
			base.mutate( mutationflags, mutationstrength ); 
		} 

		
		public override void imagedraw( cGraphics pgraphics, int drawflags ) 
		{
            /*
            if (( Edged & !Filled ) || ( (drawflags & ACView.DF_WIREFRAME) != 0 )) 
			{ 
				pgraphics.setMaterialColor( LineColor ); 
				glshape.glutWireSphere( _radius, _slices, _stacks ); 
			} 
			if ( Filled && ( (drawflags & ACView.DF_WIREFRAME) == 0 )) 
			{ 
				pgraphics.setMaterialColor( FillColor ); 
				glshape.glutSolidSphere( _radius, _slices, _stacks ); 
			} 
            */
            pgraphics.setMaterialColor(LineColor);
            glshape.glutWireSphere(_radius, _slices, _stacks);
            if ( Filled )
            {
                pgraphics.setMaterialColor(FillColor);
                glshape.glutSolidSphere(_radius, _slices, _stacks);
            }

        }


    } 
	
}
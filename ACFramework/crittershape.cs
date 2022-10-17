using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACFramework
{
    class cCritterShape : cCritter
    {
        protected float collideRadius = 1.0f;

        public cCritterShape(cGame pownergame) :
            base(pownergame)
        {
            FixedFlag = true;
            moveTo(new cVector3(_movebox.Midx, _movebox.Midy, _movebox.Midz));
            rotate(new cSpin((float)Math.PI / 2.0f, new cVector3(1.0f, 0.0f, 0.0f)));
        }

        public override bool collide(cCritter pcritter)
        {
            if (Position.distanceTo(pcritter.Position) < collideRadius)
            {
                pcritter.moveTo(pcritter.OldPosition);
                return true;
            }
            return false;
        }

        public override int collidesWith(cCritter pothercritter)
        {
            if (pothercritter is cCritter3DPlayer)
                return cCollider.COLLIDEASCALLER;
            else
                return cCollider.DONTCOLLIDE; // I'll fix this later -- JC
        }

        public float CollideRadius
        {
            set
            {
                collideRadius = value;
            }
        }
    }
}


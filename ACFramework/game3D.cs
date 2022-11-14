using System;
using System.Drawing;
using System.Windows.Forms;

// mod: setRoom1 doesn't repeat over and over again

namespace ACFramework
{ 
	
	class cCritterDoor : cCritterWall 
	{

	    public cCritterDoor(cVector3 enda, cVector3 endb, float thickness, float height, cGame pownergame ) 
		    : base( enda, endb, thickness, height, pownergame ) 
	    { 
	    }
		
		public override bool collide( cCritter pcritter ) 
		{ 
			bool collided = base.collide( pcritter ); 
			if ( collided && pcritter is cCritter3DPlayer ) 
			{ 
				(( cGame3D ) Game ).setdoorcollision( ); 
				return true; 
			} 
			return false; 
		}

        public override string RuntimeClass
        {
            get
            {
                return "cCritterDoor";
            }
        }
	}

	//created wall to move
	class cCritterMovingWall : cCritterWall
	{

		public cCritterMovingWall(cVector3 enda, cVector3 endb, float thickness, float height, cGame pownergame)
			: base(enda, endb, thickness, height, pownergame)
		{
		}

		public override bool collide(cCritter pcritter)
		{
			bool collided = base.collide(pcritter);
			if (collided && pcritter is cCritter3DPlayer)
			{
				return true;
			}
			return false;
		}
        public override void update(ACView pactiveview, float dt)
        {
            base.update(pactiveview, dt);
			
			rotate(new cSpin(((float)Math.PI) / 480.0f, new cVector3(0.0f, 1.0f, 0.0f)));
		}
        public override string RuntimeClass
		{
			get
			{
				return "cCritterMovingWall";
			}
		}
	}

	class cCritterLavaWall : cCritterWall
	{

		public cCritterLavaWall(cVector3 enda, cVector3 endb, float thickness, float height, cGame pownergame)
			: base(enda, endb, thickness, height, pownergame)
		{
		}

		public override bool collide(cCritter pcritter)
		{
			bool collided = base.collide(pcritter);
			if (collided && pcritter is cCritter3DPlayer)
			{
				Player.setHealth(-1);

				return true;
			}
			return false;
		}
		public override void update(ACView pactiveview, float dt)
		{
			base.update(pactiveview, dt);

			
		}
		public override string RuntimeClass
		{
			get
			{
				return "cCritterLavaWall";
			}
		}
	}

	class cCritterDanceWall : cCritterWall
	{
		
		public cCritterDanceWall(cVector3 enda, cVector3 endb, float thickness, float height, cGame pownergame)
			: base(enda, endb, thickness, height, pownergame)
		{
		}

		public override bool collide(cCritter pcritter)
		{
			bool collided = base.collide(pcritter);
			if (collided && pcritter is cCritter3DPlayer)
			{
                if (!cGame3D.setListener) 
				{
					Player.Listener = new cListenerDance();
					MessageBox.Show("Use the movement keys to \nincrease your score to 100 with funky fresh moves");
				}
				cGame3D.setListener = true;
				return true;
			}
			return false;
		}
		public override void update(ACView pactiveview, float dt)
		{
			base.update(pactiveview, dt);


		}
		public override string RuntimeClass
		{
			get
			{
				return "cCritterDanceWall";
			}
		}
	}


	//==============Critters for the cGame3D: Player, Ball, Treasure ================ 

	class cCritter3DPlayer : cCritterArmedPlayer 
	{ 
        private bool warningGiven = false;
		
        public cCritter3DPlayer( cGame pownergame ) 
            : base( pownergame ) 
		{ 
			BulletClass = new cCritter3DPlayerBulletMario( );
			Sprite = new cSpriteQuake(ModelsMD2.Yoshi);
			Sprite.SpriteAttitude = cMatrix3.scale( 2, 0.8f, 0.4f ); 
			setRadius( 0.45f ); //Default cCritter.PLAYERRADIUS is 0.4.  
			setHealth( 10 ); 
			moveTo( _movebox.LoCorner + new cVector3( 0.0f, 0.0f, 2.0f )); 
			WrapFlag = cCritter.CLAMP; //Use CLAMP so you stop dead at edges.
			Armed = true; //Let's use bullets.
			MaxSpeed =  cGame3D.MAXPLAYERSPEED; 
			AbsorberFlag = true; //Keeps player from being buffeted about.
			ListenerAcceleration = 160.0f; //So Hopper can overcome gravity.  Only affects hop.

			// YHopper hop strength 12.0
			//Listener = new cListenerScooterLevitator();
			Listener = new cListenerQuakeScooterYHopper( 0.2f, 12.0f ); 
            // the two arguments are walkspeed and hop strength -- JC
            
            addForce( new cForceGravity( 50.0f )); /* Uses  gravity. Default strength is 25.0.
			Gravity	will affect player using cListenerHopper. */ 
			AttitudeToMotionLock = false; //It looks nicer is you don't turn the player with motion.
			Attitude = new cMatrix3( new cVector3(0.0f, 0.0f, -1.0f), new cVector3( -1.0f, 0.0f, 0.0f ), 
                new cVector3( 0.0f, 1.0f, 0.0f ), Position); 
		}

        public override void update(ACView pactiveview, float dt)
        {
            base.update(pactiveview, dt); //Always call this first
            //if (distanceTo(new cVector3(0.0f, Game.Border.Loy,0.0f)) <= 5.0f)
            //{
                
              //  MessageBox.Show("Lava.");
            //}
			
 
        } 

        public override bool collide( cCritter pcritter ) 
		{ 
			bool playerhigherthancritter = Position.Y - Radius > pcritter.Position.Y; 
		/* If you are "higher" than the pcritter, as in jumping on it, you get a point
	and the critter dies.  If you are lower than it, you lose health and the
	critter also dies. To be higher, let's say your low point has to higher
	than the critter's center. We compute playerhigherthancritter before the collide,
	as collide can change the positions. */
            _baseAccessControl = 1;
			bool collided = base.collide( pcritter );
            _baseAccessControl = 0;
            if (!collided) 
				return false;
		/* If you're here, you collided.  We'll treat all the guys the same -- the collision
	 with a Treasure is different, but we let the Treasure contol that collision. */ 
			if ( playerhigherthancritter ) 
			{
                Framework.snd.play(Sound.Goopy); 
				addScore( 10 ); 
			} 
			else 
			{ 
				damage( 1 );
                Framework.snd.play(Sound.Crunch); 
			} 
			pcritter.die(); 
			return true; 
		}

        public override cCritterBullet shoot()
        {
            Framework.snd.play(Sound.Goopy);
            return base.shoot();
            
        }



        public override string RuntimeClass
        {
            get
            {
                return "cCritter3DPlayer";
            }
        }
	} 
	
   
	class cCritter3DPlayerBullet : cCritterBullet 
	{

        public cCritter3DPlayerBullet() { }

        public override cCritterBullet Create()
            // has to be a Create function for every type of bullet -- JC
        {
            return new cCritter3DPlayerBullet();
        }
		
		public override void initialize( cCritterArmed pshooter ) 
		{ 
			base.initialize( pshooter );
			Sprite = new cSpriteQuake(ModelsMD2.Yoshi);
			//Sprite.FillColor = Color.Crimson;
            // can use setSprite here too
            setRadius(0.1f);
		} 

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3DPlayerBullet";
            }
        }
	}

	class cCritter3DPlayerBulletMario : cCritterBullet
	{

		public cCritter3DPlayerBulletMario() { }

		public override cCritterBullet Create()
		// has to be a Create function for every type of bullet -- JC
		{
			return new cCritter3DPlayerBulletMario();
		}

		public override void initialize(cCritterArmed pshooter)
		{
			base.initialize(pshooter);
			Sprite = new cSpriteQuake(ModelsMD2.Mario);
			//Sprite.FillColor = Color.Crimson;
			// can use setSprite here too
			setRadius(0.1f);
		}

		public override string RuntimeClass
		{
			get
			{
				return "cCritter3DPlayerBullet";
			}
		}
	}
	class cCritter3Dcharacter : cCritter  
	{ 
		
        public cCritter3Dcharacter( cGame pownergame ) 
            : base( pownergame ) 
		{
            addForce(new cForceGravity(25.0f, new cVector3( 0.0f, -1, 0.00f ))); 
			addForce( new cForceDrag( 20.0f ) );  // default friction strength 0.5 
			Density = 2.0f; 
			MaxSpeed = 30.0f;
            if (pownergame != null) //Just to be safe.
                Sprite = new cSpriteQuake(Framework.models.selectRandomCritter());
            
            // example of setting a specific model
            // setSprite(new cSpriteQuake(ModelsMD2.Knight));
            
            if ( Sprite is cSpriteQuake ) //Don't let the figurines tumble.  
			{ 
				AttitudeToMotionLock = false;   
				Attitude = new cMatrix3( new cVector3( 0.0f, 0.0f, 1.0f ), 
                    new cVector3( 1.0f, 0.0f, 0.0f ), 
                    new cVector3( 0.0f, 1.0f, 0.0f ), Position); 
				/* Orient them so they are facing towards positive Z with heads towards Y. */ 
			} 
			Bounciness = 0.0f; //Not 1.0 means it loses a bit of energy with each bounce.
			setRadius( 1.0f );
            MinTwitchThresholdSpeed = 4.0f; //Means sprite doesn't switch direction unless it's moving fast 
			randomizePosition( new cRealBox3( new cVector3( _movebox.Lox, _movebox.Loy, _movebox.Loz + 4.0f), 
				new cVector3( _movebox.Hix, _movebox.Loy, _movebox.Midz - 1.0f))); 
				/* I put them ahead of the player  */ 
			randomizeVelocity( 0.0f, 30.0f, false ); 

                        
			if ( pownergame != null ) //Then we know we added this to a game so pplayer() is valid 
				addForce( new cForceObjectSeek( Player, 0.5f ));

            int begf = Framework.randomOb.random(0, 171);
            int endf = Framework.randomOb.random(0, 171);

            if (begf > endf)
            {
                int temp = begf;
                begf = endf;
                endf = temp;
            }

			Sprite.setstate( State.Other, begf, endf, StateType.Repeat );


            _wrapflag = cCritter.BOUNCE;

		} 

		
		public override void update( ACView pactiveview, float dt ) 
		{ 
			base.update( pactiveview, dt ); //Always call this first
			if ( (_outcode & cRealBox3.BOX_HIZ) != 0 ) /* use bitwise AND to check if a flag is set. */ 
				delete_me(); //tell the game to remove yourself if you fall up to the hiz.
        } 

		// do a delete_me if you hit the left end 
	
		public override void die() 
		{ 
			Player.addScore( Value ); 
			base.die(); 
		} 

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3Dcharacter";
            }
        }
	} 
	
	class cCritterTreasure : cCritter 
	{   // Try jumping through this hoop
		
		public cCritterTreasure( cGame pownergame ) : 
		base( pownergame ) 
		{ 
			/* The sprites look nice from afar, but bitmap speed is really slow
		when you get close to them, so don't use this. */ 
			cShape ppoly = new cShape( 24 ); 
			ppoly.Filled = false;
            ppoly.LineColor = Color.LightGray;
			ppoly.LineWidthWeight = 0.5f;
			Sprite = ppoly; 
			_collidepriority = cCollider.CP_PLAYER + 1; /* Let this guy call collide on the
			player, as his method is overloaded in a special way. */ 
			rotate( new cSpin( (float) Math.PI / 2.0f, new cVector3(0.0f, 0.0f, 1.0f) )); /* Trial and error shows this
			rotation works to make it face the z diretion. */ 
			setRadius( cGame3D.TREASURERADIUS ); 
			FixedFlag = true;
            moveTo(new cVector3(_movebox.Midx, _movebox.Midy - 2.0f,
                _movebox.Loz - 1.5f * cGame3D.TREASURERADIUS));
		} 

		
		public override bool collide( cCritter pcritter ) 
		{ 
			if ( contains( pcritter )) //disk of pcritter is wholly inside my disk 
			{
                Framework.snd.play(Sound.Clap); 
				pcritter.addScore( 100 ); 
				pcritter.addHealth( 1 ); 
				pcritter.moveTo( new cVector3( _movebox.Midx, _movebox.Loy + 1.0f,
                    _movebox.Hiz - 3.0f )); 
				return true; 
			} 
			else 
				return false; 
		} 

		//Checks if pcritter inside.
	
		public override int collidesWith( cCritter pothercritter ) 
		{ 
			if ( pothercritter is cCritter3DPlayer ) 
				return cCollider.COLLIDEASCALLER; 
			else 
				return cCollider.DONTCOLLIDE; 
		} 

		/* Only collide
			with cCritter3DPlayer. */ 

        public override string RuntimeClass
        {
            get
            {
                return "cCritterTreasure";
            }
        }
	}

	class cCritterShooter : cCritterArmed
	{   // Try jumping through this hoop

		public cCritterShooter(cGame pownergame) :
		base(pownergame)
		{
			/* The sprites look nice from afar, but bitmap speed is really slow
		when you get close to them, so don't use this. */
			_collidepriority = cCollider.CP_PLAYER + 1;
			Sprite = new cSpriteQuake(ModelsMD2.Carmack);
			Sprite.ModelState = State.Idle;
			FixedFlag = false;
			Armed = true;
			_bshooting = true;
			Attitude = new cMatrix3(new cVector3(0.0f, 0.0f, 1.0f),
					new cVector3(1.0f, 0.0f, 0.0f),
					new cVector3(0.0f, 1.0f, 0.0f), Position);

		}


		public override bool collide(cCritter pcritter)
		{
			if (contains(pcritter)) //disk of pcritter is wholly inside my disk 
			{
				Framework.snd.play(Sound.Clap);
				
				pcritter.addHealth(-10);
				MessageBox.Show("Congrats, you walked into an immobile object! \nLiterally the only way you could have lost in this room");
				return true;
			}
			else
				return false;
		}

		//Checks if pcritter inside.

		public override int collidesWith(cCritter pothercritter)
		{
			if (pothercritter is cCritter3DPlayer)
				return cCollider.COLLIDEASCALLER;
			else
				return cCollider.DONTCOLLIDE;
		}

		/* Only collide
			with cCritter3DPlayer. */

		public override string RuntimeClass
		{
			get
			{
				return "cCritterShooter";
			}
		}
	}

	class cCritterTarget : cCritter3Dcharacter
	{   // Try jumping through this hoop

		public cCritterTarget(cGame pownergame) :
		base(pownergame)
		{
			/* The sprites look nice from afar, but bitmap speed is really slow
		when you get close to them, so don't use this. */
			_collidepriority = cCollider.CP_PLAYER + 1;
			Sprite = new cSpriteQuake(ModelsMD2.Chicken);
			setRadius(cGame3D.CRITTERMAXRADIUS);
			FixedFlag = false;
			MaxSpeed = 0.0f;
		}


		

		
		public override string RuntimeClass
		{
			get
			{
				return "cCritterTarget";
			}
		}
	}

	class cCritterDanceBoss : cCritter3Dcharacter
	{   // Try jumping through this hoop

		public cCritterDanceBoss(cGame pownergame) :
		base(pownergame)
		{
			/* The sprites look nice from afar, but bitmap speed is really slow
		when you get close to them, so don't use this. */
			_collidepriority = cCollider.CP_PLAYER + 1;
			Sprite = new cSpriteQuake(ModelsMD2.Luigi);
			setRadius(cGame3D.CRITTERMAXRADIUS);
			FixedFlag = false;
			MaxSpeed = 0.0f;
		}

		public override bool collide(cCritter pcritter)
		{
			if (contains(pcritter)) //disk of pcritter is wholly inside my disk 
			{
				Framework.snd.play(Sound.Clap);

				pcritter.addHealth(-10);
				MessageBox.Show("Congrats, you walked into an immobile object! \nLiterally the only way you could have lost in this room");
				return true;
			}
			else
				return false;
		}



		public override string RuntimeClass
		{
			get
			{
				return "cCritterDanceBoss";
			}
		}
	}
	//======================cGame3D========================== 

	class cGame3D : cGame 
	{ 
		public static readonly float TREASURERADIUS = 1.2f; 
		public static readonly float WALLTHICKNESS = 0.5f; 
		public static readonly float PLAYERRADIUS = 0.2f; 
		public static readonly float MAXPLAYERSPEED = 30.0f; 
		private cCritterTreasure _ptreasure;
        private cCritterShape shape;
        private bool doorcollision;
        private bool wentThrough = false;
        private float startNewRoom;
		public static int roomCount = 0;
		public static bool roomLock = false;
		public static bool sentMessage = false;
		public static bool HarpSound = false;
		public static bool setListener = false;

		public cGame3D() 
		{
			doorcollision = false; 
			_menuflags &= ~ cGame.MENU_BOUNCEWRAP; 
			_menuflags |= cGame.MENU_HOPPER; //Turn on hopper listener option.
			_spritetype = cGame.ST_MESHSKIN; 
			setBorder( 64.0f, 16.0f, 64.0f ); // size of the world
		
			cRealBox3 skeleton = new cRealBox3();
            skeleton.copy(_border);
			setSkyBox( skeleton );
		/* In this world the coordinates are screwed up to match the screwed up
		listener that I use.  I should fix the listener and the coords.
		Meanwhile...
		I am flying into the screen from HIZ towards LOZ, and
		LOX below and HIX above and
		LOY on the right and HIY on the left. */ 
			SkyBox.setSideTexture( cRealBox3.HIZ, BitmapRes.volcanoWall); //Make the near HIZ transparent 
			SkyBox.setSideTexture( cRealBox3.LOZ, BitmapRes.volcanoWall); //Far wall 
			SkyBox.setSideTexture( cRealBox3.LOX, BitmapRes.volcanoWall ); //left wall 
            SkyBox.setSideTexture( cRealBox3.HIX, BitmapRes.volcanoWall ); //right wall 
			SkyBox.setSideTexture( cRealBox3.LOY, BitmapRes.lava); //floor 
			SkyBox.setSideTexture( cRealBox3.HIY, BitmapRes.Sky ); //ceiling 
		
			WrapFlag = cCritter.BOUNCE; 
			_seedcount = 0; 
			setPlayer( new cCritter3DPlayer( this )); 
			//_ptreasure = new cCritterTreasure( this );
            //shape = new cCritterShape(this);
            //shape.Sprite = new cSphere( 3, Color.DarkBlue );
            //shape.moveTo(new cVector3( Border.Midx, Border.Hiy, Border.Midz ));

			/* In this world the x and y go left and up respectively, while z comes out of the screen.
		A wall views its "thickness" as in the y direction, which is up here, and its
		"height" as in the z direction, which is into the screen. */ 
			//First draw a wall with dy height resting on the bottom of the world.
			float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */ 
			float height = 0.1f * _border.YSize; 
			float ycenter = -_border.YRadius + height / 2.0f; 
			float wallthickness = cGame3D.WALLTHICKNESS;


            cCritterMovingWall pwall = new cCritterMovingWall( 
				new cVector3( _border.Midx + 2.0f, ycenter, zpos ), 
				new cVector3( _border.Hix, ycenter, zpos ), 
				height, //thickness param for wall's dy which goes perpendicular to the 
					//baseline established by the frist two args, up the screen 
				wallthickness, //height argument for this wall's dz  goes into the screen 
				this );
			cSpriteTextureBox pspritebox = 
				new cSpriteTextureBox( pwall.Skeleton, BitmapRes.Wall3, 16 ); //Sets all sides 
				/* We'll tile our sprites three times along the long sides, and on the
			short ends, we'll only tile them once, so we reset these two. */
          pwall.Sprite = pspritebox;


			//-------------------create a new platform-----------------------------------------------------------

			//beginning platform in bottom corner
			cCritterWall pwall1 = new cCritterWall(
			   new cVector3(_border.Midx + 15.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.8f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-4.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox psprit =
				new cSpriteTextureBox(pwall1.Skeleton, BitmapRes.lava, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pwall1.Sprite = psprit;
			//floor coordinate -7.6
			pwall1.moveTo(new cVector3(-27.6f, -7.6f, 29.6f));

			//another platform-----------------------------------------------------------------------------------

			cCritterWall pwall3 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 1.0f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-6.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox psprit3 =
				new cSpriteTextureBox(pwall3.Skeleton, BitmapRes.lava, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pwall3.Sprite = psprit3;
			//floor coordinate -7.6
			pwall3.moveTo(new cVector3(-19.6f, -6.3f, 29.0f));

			//a log-----------------------------------------------------------------------------------

			cCritterWall plog1 = new cCritterWall(
			   new cVector3(_border.Midx + 15.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.5f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-2.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox pspritlog =
				new cSpriteTextureBox(plog1.Skeleton, BitmapRes.treeBark, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			plog1.Sprite = pspritlog;
			//floor coordinate -7.6
			plog1.moveTo(new cVector3(-11.6f, -5.6f, 30.0f));


			//another platform4-----------------------------------------------------------------------------------

			cCritterWall pwall4 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.5f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-6.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox psprit4 =
				new cSpriteTextureBox(pwall4.Skeleton, BitmapRes.lava, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pwall4.Sprite = psprit4;
			pwall4.moveTo(new cVector3(-10.5f, -3.2f, 28.4f));

			//ramp1------------------------------------------------------------

			cCritterWall pramp1 = new cCritterWall(
				//subtracting/adding from ycenter slants the platform
				//zpos rotates it horizontally 
			   new cVector3(_border.Midx + 15.0f, ycenter - 2.0f, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 2.0f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-2.5f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox pspritramp =
				new cSpriteTextureBox(pramp1.Skeleton, BitmapRes.treeBark, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pramp1.Sprite = pspritramp;
			//pramp1.rotate(new cSpin(((float)Math.PI)/4.0f,
			//	new cVector3(1.0f, 0.0f, 0.0f)));
			//floor coordinate -7.6
			pramp1.moveTo(new cVector3(1.0f, -2.2f, 28.0f));

			//another platform5-----------------------------------------------------------------------------------

			cCritterWall pwall5 = new cCritterWall(
			   new cVector3(_border.Midx + 18.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 20.0f, ycenter, zpos),
			   height - 0.5f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-6.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox psprit5 =
				new cSpriteTextureBox(pwall5.Skeleton, BitmapRes.lava, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pwall5.Sprite = psprit5;
			pwall5.moveTo(new cVector3(6.5f, -4.8f, 21.0f));

			//another platform6----------------------------------------------------------------------------------

			cCritterWall pwall6 = new cCritterWall(
				//making the Midx and Hix values closer to each other's opposite makes the platform smaller/bigger
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-6.0f), //height argument for this wall's dz  goes into the screen 
			   this);
				cSpriteTextureBox psprit6 =
					new cSpriteTextureBox(pwall6.Skeleton, BitmapRes.lava, 16); //Sets all sides 
				/* We'll tile our sprites three times along the long sides, and on the
			short ends, we'll only tile them once, so we reset these two. */
				pwall6.Sprite = psprit6;
				pwall6.moveTo(new cVector3(-27.6f, -5.6f, 22.6f));

			//another platform7----------------------------------------------------------------------------------

			cCritterWall pwall7 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-5.0f), //height argument for this wall's dz  goes into the screen 
			   this);
						cSpriteTextureBox psprit7 =
							new cSpriteTextureBox(pwall7.Skeleton, BitmapRes.lava, 16); //Sets all sides 
						/* We'll tile our sprites three times along the long sides, and on the
					short ends, we'll only tile them once, so we reset these two. */
						pwall7.Sprite = psprit7;
						pwall7.moveTo(new cVector3(-27.6f, -3.6f, 14.6f));
			//plog2---------------------------------------------------------------------------------------------

			cCritterWall plog2 = new cCritterWall(
			   new cVector3(_border.Midx + 14.8f, ycenter, zpos),
			   new cVector3(_border.Hix - 14.7f, ycenter, zpos),
			   height - 0.5f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-10.0f), //height argument for this wall's dz  goes into the screen 
			   this);
						cSpriteTextureBox pspritlog2 =
							new cSpriteTextureBox(plog2.Skeleton, BitmapRes.treeBark, 16); //Sets all sides 
						/* We'll tile our sprites three times along the long sides, and on the
					short ends, we'll only tile them once, so we reset these two. */
						plog2.Sprite = pspritlog2;
						
						//floor coordinate -7.6
						plog2.moveTo(new cVector3(-26.6f, -4.9f, 5.8f));
			//another platform8------------------------------------------------------------------------------------------

			cCritterWall pwall8 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-5.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox psprit8 =
				new cSpriteTextureBox(pwall8.Skeleton, BitmapRes.lava, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pwall8.Sprite = psprit8;
			pwall8.moveTo(new cVector3(-27.0f, -3.6f, -3.2f));

			//another ramp2---------------------------------------------------------------------------------------------

			cCritterWall pramp2 = new cCritterWall(
			   new cVector3(_border.Midx + 14.8f, ycenter, zpos),
			   new cVector3(_border.Hix - 14.7f, ycenter, zpos),
			   height - 1.0f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-7.0f), //height argument for this wall's dz  goes into the screen 
			   this);
					cSpriteTextureBox pspritramp2 =
						new cSpriteTextureBox(pramp2.Skeleton, BitmapRes.treeBark, 16); //Sets all sides 
					/* We'll tile our sprites three times along the long sides, and on the
				short ends, we'll only tile them once, so we reset these two. */
					pramp2.Sprite = pspritramp2;
					pramp2.rotate(new cSpin(((float)Math.PI) / 11.0f,
						new cVector3(1.0f, 0.0f, 0.0f)));
			//floor coordinate -7.6
			pramp2.moveTo(new cVector3(-26.6f, -2.2f, -10.8f));

			//another platform9----------------------------------------------------------------------------------------

			cCritterWall pwall9 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-5.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox psprit9 =
				new cSpriteTextureBox(pwall9.Skeleton, BitmapRes.lava, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pwall9.Sprite = psprit9;
			pwall9.moveTo(new cVector3(-27.0f, -1.2f, -18.2f));

			//another platform rand1-----------------------------------------------------------------------------------------

			cCritterWall pwallrand1 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-6.0f), //height argument for this wall's dz  goes into the screen 
			   this);
				cSpriteTextureBox pspritrand =
					new cSpriteTextureBox(pwallrand1.Skeleton, BitmapRes.lava, 16); //Sets all sides 
				/* We'll tile our sprites three times along the long sides, and on the
			short ends, we'll only tile them once, so we reset these two. */
				pwallrand1.Sprite = pspritrand;
				pwallrand1.moveTo(new cVector3(-27.0f, -0.2f, -27.2f));

			//another platform10------------------------------------------------------------------------------------------
			cCritterWall pwall10 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-6.0f), //height argument for this wall's dz  goes into the screen 
			   this);
				cSpriteTextureBox psprit10 =
					new cSpriteTextureBox(pwall10.Skeleton, BitmapRes.lava, 16); //Sets all sides 
				/* We'll tile our sprites three times along the long sides, and on the
			short ends, we'll only tile them once, so we reset these two. */
				pwall10.Sprite = psprit10;
				pwall10.moveTo(new cVector3(-18.0f, -1.2f, -18.2f));

			//another log3----------------------------------------------------------------------------------------------

			//subtracting/adding from ycenter slants the platform
			//zpos rotates it horizontally 
			cCritterWall plog3 = new cCritterWall(
			   new cVector3(_border.Midx + 15.8f, ycenter, zpos -11.0f),
			   new cVector3(_border.Hix - 10.7f, ycenter , zpos ),
			   height - 0.3f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-3.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox pspritlog3 =
				new cSpriteTextureBox(plog3.Skeleton, BitmapRes.treeBark, 16); //Sets all sides
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			plog3.Sprite = pspritlog3;
			//floor coordinate -7.6
			plog3.moveTo(new cVector3(-12.6f, -2.2f, -9.8f));


			//another platform11---------------------------------------------------------------------------------------

			cCritterWall pwall11 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-4.0f), //height argument for this wall's dz  goes into the screen 
			   this);
				cSpriteTextureBox psprit11 =
					new cSpriteTextureBox(pwall11.Skeleton, BitmapRes.lava, 16); //Sets all sides 
				/* We'll tile our sprites three times along the long sides, and on the
			short ends, we'll only tile them once, so we reset these two. */
				pwall11.Sprite = psprit11;
				pwall11.moveTo(new cVector3(-6.5f, 1.2f, -1.4f));

			//another platform12--------------------------------------------------------------------------------------
			cCritterWall pwall12 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-4.0f), //height argument for this wall's dz  goes into the screen 
			   this);
				cSpriteTextureBox psprit12 =
					new cSpriteTextureBox(pwall12.Skeleton, BitmapRes.lava, 16); //Sets all sides 
				/* We'll tile our sprites three times along the long sides, and on the
			short ends, we'll only tile them once, so we reset these two. */
				pwall12.Sprite = psprit12;
				pwall12.moveTo(new cVector3(-6.5f, -1.2f, 5.4f));


			//another platform13----the one that should move----------------------------------------------------------------------------------
			cCritterWall pwall13 = new cCritterWall(
			   new cVector3(_border.Midx + 17.0f, ycenter, zpos),
			   new cVector3(_border.Hix - 10.0f, ycenter, zpos),
			   height - 0.6f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-4.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox psprit13 =
				new cSpriteTextureBox(pwall13.Skeleton, BitmapRes.lava, 16); //Sets all sides 
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pwall13.Sprite = psprit13;
			pwall13.moveTo(new cVector3(1.5f, -2.2f, 11.4f));

			//another ramp3----------------------------------------------------------------------------------------------

			//subtracting/adding from ycenter slants the platform
			//zpos rotates it horizontally 
			cCritterWall pramp3 = new cCritterWall(
			   new cVector3(_border.Midx + 14.8f, ycenter, zpos - 10.0f),
			   new cVector3(_border.Hix - 14.7f, ycenter, zpos),
			   height - 1.0f, //thickness param for wall's dy which goes perpendicular to the 
							  //baseline established by the frist two args, up the screen 
			   wallthickness + (-2.0f), //height argument for this wall's dz  goes into the screen 
			   this);
			cSpriteTextureBox pspritramp3 =
				new cSpriteTextureBox(pramp3.Skeleton, BitmapRes.treeBark, 16); //Sets all sides
			/* We'll tile our sprites three times along the long sides, and on the
		short ends, we'll only tile them once, so we reset these two. */
			pramp3.Sprite = pspritramp3;
			//floor coordinate -7.6
			pramp3.moveTo(new cVector3(1.6f, -2.2f, 15.8f));



			//Then draw a ramp to the top of the wall.  Scoot it over against the right wall.
			/*float planckwidth = 0.75f * height; 
			pwall = new cCritterWall( 
				new cVector3( _border.Hix -planckwidth / 2.0f, _border.Loy, _border.Hiz - 2.0f), 
				new cVector3( _border.Hix - planckwidth / 2.0f, _border.Loy + height, zpos ), 
				planckwidth, //thickness param for wall's dy which is perpenedicualr to the baseline, 
						//which goes into the screen, so thickness goes to the right 
				wallthickness, //_border.zradius(),  //height argument for wall's dz which goes into the screen 
				this );
            cSpriteTextureBox stb = new cSpriteTextureBox(pwall.Skeleton, 
                BitmapRes.Wood2, 2 );
            pwall.Sprite = stb;
			*/
			cCritterDoor pdwall = new cCritterDoor( 
				new cVector3( _border.Lox, _border.Loy, _border.Midz ), 
				new cVector3( _border.Lox, _border.Midy - 3, _border.Midz ), 
				0.1f, 2, this ); 
			cSpriteTextureBox pspritedoor = 
				new cSpriteTextureBox( pdwall.Skeleton, BitmapRes.Door ); 
			pdwall.Sprite = pspritedoor;

			/*cCritterDoor pdwall2 = new cCritterDoor(
				new cVector3(_border.Lox+3, _border.Loy, _border.Midz),
				new cVector3(_border.Lox+3, _border.Midy - 3, _border.Midz),
				0.1f, 2, this);
			cSpriteTextureBox pspritedoor2 =
				new cSpriteTextureBox(pdwall2.Skeleton, BitmapRes.Sky);
			pdwall2.Sprite = pspritedoor2;
			*/
			Player.moveTo(new cVector3(-27f, -6f, 30));
			
		}

		public void setRoom1( )
        {
            Biota.purgeCritters<cCritterWall>();
            Biota.purgeCritters<cCritter3Dcharacter>();
            Biota.purgeCritters<cCritterShape>();
			Biota.purgeCritters<cCritterShooter>();
            setBorder(50.0f, 15.0f, -20.0f); 
	        cRealBox3 skeleton = new cRealBox3();
            skeleton.copy( _border );
	        setSkyBox(skeleton);
	        SkyBox.setAllSidesTexture( BitmapRes.Wall3, 2 );
	        SkyBox.setSideTexture( cRealBox3.LOY, BitmapRes.Wood2 );
	        SkyBox.setSideTexture( cRealBox3.HIY, BitmapRes.Graphics2);
	        _seedcount = 0;
	        Player.setMoveBox( new cRealBox3( 50.0f, 15.0f, -20.0f ) );
			//Player.rotate(new cSpin(90.0f, new cVector3(1.1f)));
			Player.moveTo(new cVector3(+30.0f, 0.0f, 0.0f));

			cCritterTarget chicken1 = new cCritterTarget(this);
			cCritterTarget chicken2 = new cCritterTarget(this);
			cCritterTarget chicken3 = new cCritterTarget(this);

			

			cCritterShooter carmac1 = new cCritterShooter(this);
			cCritterShooter carmac2 = new cCritterShooter(this);

            carmac1.moveTo(new cVector3(10.0f, -15.0f, 7.0f));
			carmac1.rotate(new cSpin(new cVector3(0.0f,4.7f,0.0f)));

            carmac2.moveTo(new cVector3(10.0f, -15.0f, -7.0f));
            carmac2.rotate(new cSpin(new cVector3(0.0f, 4.7f, 0.0f)));

            chicken1.moveTo(new cVector3(-20.0f, -15.0f, 7.0f));
            chicken2.moveTo(new cVector3(-20.0f, -15.0f, -7.0f));
            chicken3.moveTo(new cVector3(-20.0f, -15.0f, 0.0f));
            //chicken1.moveTo

            //moves the wall right to left-----------------------------------------------------------------------
            float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
            float height = 0.1f * _border.YSize;
            float ycenter = -_border.YRadius + height / 2.0f;
            float wallthickness = cGame3D.WALLTHICKNESS;
       //------------------------------------------------------------------------------------ 
			cCritterWall pwall = new cCritterWall(
                new cVector3(_border.Midx - 25.0f, ycenter, zpos + 4.0f),
                new cVector3(_border.Hix - 20.0f, ycenter, zpos + 4.0f),
                height, //thickness param for wall's dy which goes perpendicular to the 
                //baseline established by the frist two args, up the screen 
                wallthickness, //height argument for this wall's dz  goes into the screen 
                this);
            cSpriteTextureBox pspritebox =
                new cSpriteTextureBox(pwall.Skeleton, BitmapRes.Concrete); //Sets all sides 

     //----------------------------------------------------------------------------------------------
            cCritterWall pwall_1 = new cCritterWall(
                            new cVector3(_border.Midx-25.0f , ycenter, zpos - 4.0f),
                            new cVector3(_border.Hix-20.0f, ycenter, zpos - 4.0f),
                            height, //thickness param for wall's dy which goes perpendicular to the 
                                    //baseline established by the frist two args, up the screen 
                            wallthickness, //height argument for this wall's dz  goes into the screen 
                            this);
            cSpriteTextureBox pspritebox1 =
                new cSpriteTextureBox(pwall_1.Skeleton, BitmapRes.Concrete); //Sets all sides 

            //----------------------------------------------------------------------------------------------
            cCritterWall pwall_2 = new cCritterWall(
                            new cVector3( 5.0f,-8.0f, 10.0f),
                            new cVector3( 5.01f, -8.0f, -10.0f),

                            //_border.Midx,ycenter,zpos
                            //_border.Hix,ycenter,zpos

                            height, //thickness param for wall's dy which goes perpendicular to the 
                                    //baseline established by the frist two args, up the screen 
                            wallthickness, //height argument for this wall's dz  goes into the screen 
                            this);
            cSpriteTextureBox pspritebox2 =
                new cSpriteTextureBox(pwall_1.Skeleton, BitmapRes.Wood2); //Sets all sides 


            //----------------------------------------------------------------------------------------------
            cCritterDoor pdwall = new cCritterDoor(
				 new cVector3(_border.Lox + 50f, _border.Loy, _border.Midz+3),
				 new cVector3(_border.Lox + 50f , _border.Midy-3.5f, _border.Midz+3),
				 0.5f, 2, this);
			cSpriteTextureBox pspritedoor =
				new cSpriteTextureBox(pdwall.Skeleton, BitmapRes.Door);
			pdwall.Sprite = pspritedoor;

			pwall.Sprite = pspritebox;
            wentThrough = true;
            startNewRoom = Age;

		//----------------------------------------------------------------------------
		
			
        }

//-----------------------------------------------------------------------------------------
		public void setRoom2()
		{
			Biota.purgeCritters<cCritterWall>();
			Biota.purgeCritters<cCritter3Dcharacter>();
			Biota.purgeCritters<cCritterShape>();
			Biota.purgeCritters<cCritterShooter>();
			setBorder(64.0f, 15.0f, 64.0f);
			cRealBox3 skeleton = new cRealBox3();
			skeleton.copy(_border);

            shape = new cCritterShape(this);
            shape.Sprite = new cSphere(3, Color.Silver);
            shape.moveTo(new cVector3(Border.Midx, Border.Hiy, Border.Midz));

			cCritterDanceBoss Luigi1 = new cCritterDanceBoss(this);
			Luigi1.moveTo(new cVector3(3f, -7, 3f));
			
			setSkyBox(skeleton);
			SkyBox.setAllSidesTexture(BitmapRes.rollerRink);
			SkyBox.setSideSolidColor(cRealBox3.LOY, Color.Black);
			SkyBox.setSideSolidColor(cRealBox3.HIY, Color.Blue);
			_seedcount = 0; ; ;
			Player.setMoveBox(new cRealBox3(64.0f, 15.0f, 64.0f));
			float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
			float height = 0.1f * _border.YSize;
			float ycenter = -_border.YRadius + height / 2.0f;
			float wallthickness = cGame3D.WALLTHICKNESS;

			cCritterDanceWall pwall = new cCritterDanceWall(
				new cVector3(_border.Midx + 50.0f , ycenter, zpos),
				new cVector3(_border.Hix - 20.0f, ycenter, zpos),
				height -1.0f , //thickness param for wall's dy which goes perpendicular to the 
						//baseline established by the frist two args, up the screen 
				wallthickness -20.0f, //height argument for this wall's dz  goes into the screen 
				this);
			cSpriteTextureBox pspritebox =
				new cSpriteTextureBox(pwall.Skeleton, BitmapRes.danceFloor); //Sets all sides
			pwall.moveTo(new cVector3(0, -70.0f , 0)); 														   //

			/* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */
			pwall.Sprite = pspritebox;
			wentThrough = true;
			startNewRoom = Age;
		}


		public override void seedCritters() 
		{
			Biota.purgeCritters<cCritterBullet>(); 
			//Biota.purgeCritters<cCritter3Dcharacter>();
            for (int i = 0; i < _seedcount; i++) 
				new cCritter3Dcharacter( this );
			Player.moveTo(new cVector3(-27f, -6f, 30));
			//Player.moveTo(new cVector3(0.0f, Border.Loy, Border.Hiz - 3.0f)); 
			/* We start at hiz and move towards	loz */
		} 

		
		public void setdoorcollision( ) { doorcollision = true; } 
		
		public override ACView View 
		{
            set
            {
                base.View = value; //You MUST call the base class method here.
                value.setUseBackground(ACView.FULL_BACKGROUND); /* The background type can be
			    ACView.NO_BACKGROUND, ACView.SIMPLIFIED_BACKGROUND, or 
			    ACView.FULL_BACKGROUND, which often means: nothing, lines, or
			    planes&bitmaps, depending on how the skybox is defined. */
                value.pviewpointcritter().Listener = new cListenerViewerRide();
            }
		} 

		
		public override cCritterViewer Viewpoint 
		{ 
            set
            {
			    if ( value.Listener.RuntimeClass == "cListenerViewerRide" ) 
			    { 
				    value.setViewpoint( new cVector3( 0.0f, 0.3f, -1.0f ), _border.Center); 
					//Always make some setViewpoint call simply to put in a default zoom.
				    value.zoom( 0.35f ); //Wideangle 
				    cListenerViewerRide prider = ( cListenerViewerRide )( value.Listener); 
				    prider.Offset = (new cVector3( -1.5f, 0.0f, 1.0f)); /* This offset is in the coordinate
				    system of the player, where the negative X axis is the negative of the
				    player's tangent direction, which means stand right behind the player. */ 
			    } 
			    else //Not riding the player.
			    { 
				    value.zoom( 1.0f ); 
				    /* The two args to setViewpoint are (directiontoviewer, lookatpoint).
				    Note that directiontoviewer points FROM the origin TOWARDS the viewer. */ 
				    value.setViewpoint( new cVector3( 0.0f, 0.3f, 1.0f ), _border.Center); 
			    }
            }
		} 

		/* Move over to be above the
			lower left corner where the player is.  In 3D, use a low viewpoint low looking up. */ 
	
		public override void adjustGameParameters() 
		{
		// (1) End the game if the player is dead 
			if ( (Health == 0) && !_gameover ) //Player's been killed and game's not over.
			{ 
				_gameover = true; 
				Player.addScore( _scorecorrection ); // So user can reach _maxscore  
                Framework.snd.play(Sound.Hallelujah);
                return ; 
			} 
		// (2) Also don't let the the model count diminish.
					//(need to recheck propcount in case we just called seedCritters).
			int modelcount = Biota.count<cCritter3Dcharacter>(); 
			int modelstoadd = _seedcount - modelcount; 
			for ( int i = 0; i < modelstoadd; i++) 
				new cCritter3Dcharacter( this );
			// (3) Maybe check some other conditions.


			//pwall.rotate(new cSpin(((float)Math.PI) / 4.0f, new cVector3(1.0f, 0.0f, 0.0f)));

			
			if (wentThrough && (Age - startNewRoom) > 2.0f)
            {
                Framework.snd.play(Sound.Doorcreak);
                roomCount++;
                //MessageBox.Show("You went through the door and are in Room " + (1 + roomCount));
                wentThrough = false;
            }

			if(Score >= 2 && HarpSound == false)
			{
                Framework.snd.play(Sound.Harp);
				HarpSound = true;
            }

            if (doorcollision == true)
            {
				if (roomCount == 0) {
					setRoom1();
					roomLock = true;
				}
				else if (roomCount == 1 && roomLock == false)
				{
					setRoom2();
				}
				else if (roomLock == true && sentMessage == false)
                {

					if (Player.Score >= 2)
					{
                        roomLock = false;
					}
					else
					{
						MessageBox.Show("This door is locked, finish the mission. \nGet a score of 2 or more");
						sentMessage = true;
                        Framework.snd.play(Sound.Knockdoor);
                    }
				}
				else if (roomLock == true && sentMessage == true)
				{
					if(Player.Score >= 2)
                    {
                        
                        roomLock = false;
                    }
					
				}
				else
                {
					MessageBox.Show("If you see this message, something went horribly wrong!");
				}

				doorcollision = false;
            }
			if (Score >= 100)
            {
				Framework.snd.play(Sound.Clap);
				MessageBox.Show("Congrats, you won!!!!!!!!!!!!!!!!!");
				_gameover = true;
				return;

			}
		} 
		
	} 
	
}
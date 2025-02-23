using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Players;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.AltCharacters
{
    public class PrincessIndi : CommanderKeen
    {
        public PrincessIndi(SpaceHashGrid grid, Rectangle hitbox, Direction direction) : base(hitbox, grid, direction)
        {
            Initialize();
        }

        public PrincessIndi(SpaceHashGrid grid, Rectangle hitbox, Direction direction, int lives, long points)
           : base(hitbox, grid, direction, lives, points)
        {
            Initialize();
        }

        private void Initialize()
        {
            _keenPogoRightSprites = new Image[]
            {
                Properties.Resources.princess_indi_pogo_right1,
                Properties.Resources.princess_indi_pogo_right2,
            };

            _keenPogoRight1 = _keenPogoRightSprites[0];
            _keenPogoRight2 = _keenPogoRightSprites[1];

            _keenPogoLeftSprites = new Image[]
            {
                Properties.Resources.princess_indi_pogo_left1,
                Properties.Resources.princess_indi_pogo_left2,
            };

            _keenPogoLeft1 = _keenPogoLeftSprites[0];
            _keenPogoLeft2 = _keenPogoLeftSprites[1];

            _keenRunRightSprites = new Image[]
            {
                Properties.Resources.princess_indi_run_right1,
                Properties.Resources.princess_indi_run_right2,
                Properties.Resources.princess_indi_run_right3,
                Properties.Resources.princess_indi_run_right4
            };

            _keenRunLeftSprites = new Image[]
            {
                Properties.Resources.princess_indi_run_left1,
                Properties.Resources.princess_indi_run_left2,
                Properties.Resources.princess_indi_run_left3,
                Properties.Resources.princess_indi_run_left4
            };

            _keenClimbRightSprites = new Image[]
            {
                Properties.Resources.princess_indi_wall_climb_right1,
                Properties.Resources.princess_indi_wall_climb_right2,
                Properties.Resources.princess_indi_wall_climb_right3,
                Properties.Resources.princess_indi_wall_climb_right4
            };

            _keenClimbLeftSprites = new Image[]
            {
                Properties.Resources.princess_indi_wall_climb_left1,
                Properties.Resources.princess_indi_wall_climb_left2,
                Properties.Resources.princess_indi_wall_climb_left3,
                Properties.Resources.princess_indi_wall_climb_left4
            };

            _keenClimbUpPoleRightSprites = new Image[]
            {
                Properties.Resources.princess_indi_pole_left1,
                Properties.Resources.princess_indi_pole_left2,
                Properties.Resources.princess_indi_pole_left3
            };

            _keenClimbUpPoleLeftSprites = new Image[]
            {
                Properties.Resources.princess_indi_pole_right1,
                Properties.Resources.princess_indi_pole_right2,
                Properties.Resources.princess_indi_pole_right3
            };

            _keenClimbDownPoleSprites = new Image[]
            {
                Properties.Resources.princess_indi_shimmy_down1,
                Properties.Resources.princess_indi_shimmy_down2,
                Properties.Resources.princess_indi_shimmy_down3,
                Properties.Resources.princess_indi_shimmy_down4,
            };

            _keenLookDownSprites = new Image[]
            {
                Properties.Resources.princess_indi_duck,
                Properties.Resources.princess_indi_duck
            };

            _keenDoorEnterSprites = new Image[]
            {
                Properties.Resources.princess_indi_walk_through_door1,
                Properties.Resources.princess_indi_walk_through_door2,
                Properties.Resources.princess_indi_walk_through_door3,
                Properties.Resources.princess_indi_walk_through_door4,
                Properties.Resources.princess_indi_walk_through_door5
            };

            _keenShootUpAerial = Properties.Resources.princess_indi_shoot_up_aerial;
            _keenShootDownAerial = Properties.Resources.princess_indi_shoot_down_aerial;
            _keenShootRightAerial = Properties.Resources.princess_indi_shoot_right_aerial;
            _keenShootLeftAerial = Properties.Resources.princess_indi_shoot_left_aerial;

            _keenShootUp = Properties.Resources.princess_indi_shoot_up_standing;
            _keenShootLeft = Properties.Resources.princess_indi_shoot_left_standing;
            _keenShootRight = Properties.Resources.princess_indi_shoot_right_standing;

            _keenShootDownRightPole = Properties.Resources.princess_indi_shoot_down_pole_right;
            _keenShootUpRightPole = Properties.Resources.princess_indi_shoot_up_pole_right;
            _keenShootDownLeftPole = Properties.Resources.princess_indi_shoot_down_pole_left;
            _keenShootUpLeftPole = Properties.Resources.princess_indi_shoot_up_pole_left;
            _keenShootPoleLeft = Properties.Resources.princess_indi_shoot_left_pole;
            _keenShootPoleRight = Properties.Resources.princess_indi_shoot_right_pole;

            _keenLookUp = Properties.Resources.princess_indi_look_up;
            _keenEnterDoor1 = Properties.Resources.princess_indi_walk_through_door1;
            _keenStunned = Properties.Resources.princess_indi_dead1;
            _keenStandright = Properties.Resources.princess_indi_standing_right;
            _keenStandLeft = Properties.Resources.princess_indi_standing_left;
            _keenFallRight = Properties.Resources.princess_indi_fall_right;
            _keenFallLeft = Properties.Resources.princess_indi_fall_left;
            _keenJumpLeft1 = Properties.Resources.princess_indi_jump_left1;
            _keenJumpRight1 = Properties.Resources.princess_indi_jump_right1;
            _keenHangLeft = Properties.Resources.princess_indi_wall_hang_left;
            _keenHangRight = Properties.Resources.princess_indi_wall_hang_right;

            _keenDead1 = Properties.Resources.princess_indi_dead1;
            _keenDead2 = Properties.Resources.princess_indi_dead2;

            this.UpdateSprite();
        }
    }
}

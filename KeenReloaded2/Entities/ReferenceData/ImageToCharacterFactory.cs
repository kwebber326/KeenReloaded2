using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.ReferenceData
{
    public static class ImageToCharacterFactory
    {
        static Dictionary<char, Image> _characterImgMapping;

        public static Dictionary<char, Image> CharacterImageMapping
        {
            get
            {
                if (_characterImgMapping == null)
                {
                    _characterImgMapping = new Dictionary<char, Image>()
                    {
                        { 'a', Properties.Resources.a },
                        { 'b', Properties.Resources.b },
                        { 'c', Properties.Resources.c },
                        { 'd', Properties.Resources.d },
                        { 'e', Properties.Resources.e },
                        { 'f', Properties.Resources.f },
                        { 'g', Properties.Resources.g },
                        { 'h', Properties.Resources.h },
                        { 'i', Properties.Resources.i },
                        { 'j', Properties.Resources.j },
                        { 'k', Properties.Resources.k },
                        { 'l', Properties.Resources.l },
                        { 'm', Properties.Resources.m },
                        { 'n', Properties.Resources.n },
                        { 'o', Properties.Resources.o },
                        { 'p', Properties.Resources.p },
                        { 'q', Properties.Resources.q },
                        { 'r', Properties.Resources.r },
                        { 's', Properties.Resources.s },
                        { 't', Properties.Resources.t },
                        { 'u', Properties.Resources.u },
                        { 'v', Properties.Resources.v },
                        { 'w', Properties.Resources.w },
                        { 'x', Properties.Resources.x },
                        { 'y', Properties.Resources.y },
                        { 'z', Properties.Resources.z },

                        { 'A', Properties.Resources.A_ },
                        { 'B', Properties.Resources.B_ },
                        { 'C', Properties.Resources.C_ },
                        { 'D', Properties.Resources.D_ },
                        { 'E', Properties.Resources.E_ },
                        { 'F', Properties.Resources.F_ },
                        { 'G', Properties.Resources.G_ },
                        { 'H', Properties.Resources.H_ },
                        { 'I', Properties.Resources.I_ },
                        { 'J', Properties.Resources.J_ },
                        { 'K', Properties.Resources.K_ },
                        { 'L', Properties.Resources.L_ },
                        { 'M', Properties.Resources.M_ },
                        { 'N', Properties.Resources.N_ },
                        { 'O', Properties.Resources.O_ },
                        { 'P', Properties.Resources.P_ },
                        { 'Q', Properties.Resources.Q_ },
                        { 'R', Properties.Resources.R_ },
                        { 'S', Properties.Resources.S_ },
                        { 'T', Properties.Resources.T_ },
                        { 'U', Properties.Resources.U_ },
                        { 'V', Properties.Resources.V_ },
                        { 'W', Properties.Resources.W_ },
                        { 'X', Properties.Resources.X_ },
                        { 'Y', Properties.Resources.Y_ },
                        { 'Z', Properties.Resources.Z_ },

                        { '0', Properties.Resources._0 },
                         { '1', Properties.Resources._1 },
                         { '2', Properties.Resources._2 },
                         { '3', Properties.Resources._3 },
                         { '4', Properties.Resources._4 },
                         { '5', Properties.Resources._5 },
                         { '6', Properties.Resources._6 },
                         { '7', Properties.Resources._7 },
                         { '8', Properties.Resources._8 },
                         { '9', Properties.Resources._9 },

                        { ',', Properties.Resources.sc_comma },
                        { ' ', null },
                        { '"', Properties.Resources.sc_double_quote },
                        { '&', Properties.Resources.sc_and_sign },
                        { '@', Properties.Resources.sc_at_symbol },
                        { '\\', Properties.Resources.sc_backslash },
                        { ':', Properties.Resources.sc_colon },
                        { '-', Properties.Resources.sc_dash },
                        { '$', Properties.Resources.sc_dollar_sign },
                        { '=', Properties.Resources.sc_equals },
                        { '!', Properties.Resources.sc_exclamation_point },
                        { '/', Properties.Resources.sc_forward_slash },
                        { '>', Properties.Resources.sc_greater_than_caret },
                        { '#', Properties.Resources.sc_hashtag },
                        { '[', Properties.Resources.sc_left_brace },
                        { '{', Properties.Resources.sc_left_bracket },
                        { '(', Properties.Resources.sc_left_parenthesis },
                        { '`', Properties.Resources.sc_left_single_quote },
                        { '<', Properties.Resources.sc_less_than_caret },
                        { '%', Properties.Resources.sc_percent_sign },
                        { '.', Properties.Resources.sc_period },
                        { '|', Properties.Resources.sc_pipe },
                        { '+', Properties.Resources.sc_plus },
                        { '?', Properties.Resources.sc_question_mark },
                        { ']', Properties.Resources.sc_right_brace },
                        { '}', Properties.Resources.sc_right_bracket },
                        { ')', Properties.Resources.sc_right_parenthesis },
                        { '\'', Properties.Resources.sc_right_single_quote },
                        { ';', Properties.Resources.sc_semicolon },
                        { '*', Properties.Resources.sc_star },
                        { '~', Properties.Resources.sc_tilde },
                        { '_', Properties.Resources.sc_underscore }
                    };
                }
                return _characterImgMapping;
            }
        }

    }
}

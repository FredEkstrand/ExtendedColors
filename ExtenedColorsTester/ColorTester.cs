using System;
using NUnit.VisualStudio.TestAdapter;
using NUnit.Framework;
using Ekstrand.Drawing;
using System.Drawing;

namespace ExtenedColorsTester
{
    [TestFixture]
    public class ColorTester
    {
        /* Random source values coped for testing
          values[(int)KnownExtendedColors.AlwaysApple] = unchecked((int)0xFFA8A268);
          values[(int)KnownExtendedColors.BirdHouseBrown] = unchecked((int)0xFF6E483A);
          values[(int)KnownExtendedColors.ClassicSand] = unchecked((int)0xFFD6BCAA);
          values[(int)KnownExtendedColors.EveningGlow] = unchecked((int)0xFFFFD892);
          values[(int)KnownExtendedColors.GreenSilk] = unchecked((int)0xFFA1C2B1);
          values[(int)KnownExtendedColors.HighSociety] = unchecked((int)0xFFCBB7C0);
          values[(int)KnownExtendedColors.Irresistible] = unchecked((int)0xFFE8DCC8);
          values[(int)KnownExtendedColors.LiquidBlue] = unchecked((int)0xFFA6D4DE);
          values[(int)KnownExtendedColors.MareaBaja] = unchecked((int)0xFF2E5464);
          values[(int)KnownExtendedColors.NavelOrange] = unchecked((int)0xFFEC8430);
          values[(int)KnownExtendedColors.OnlyNatural] = unchecked((int)0xFFE2D3C4);
          values[(int)KnownExtendedColors.PassionatePurple] = unchecked((int)0xFF795484);
          values[(int)KnownExtendedColors.QuaintPeche] = unchecked((int)0xFFEACDC1);
          values[(int)KnownExtendedColors.Reddish] = unchecked((int)0xFFB56966);
          values[(int)KnownExtendedColors.RookwoodAmber] = unchecked((int)0xFFC08650);
          values[(int)KnownExtendedColors.SalmonEggs] = unchecked((int)0xFFC59688);
          values[(int)KnownExtendedColors.SilverSage] = unchecked((int)0xFFD4CDB5);
          values[(int)KnownExtendedColors.SplashOfHoney] = unchecked((int)0xFFDBB78E);
          values[(int)KnownExtendedColors.SummerBreeze] = unchecked((int)0xFFD3E5DC);
          values[(int)KnownExtendedColors.Talavera] = unchecked((int)0xFFA1928B);
          values[(int)KnownExtendedColors.TrueKhaki] = unchecked((int)0xFFB9AC97);
          values[(int)KnownExtendedColors.Undercool] = unchecked((int)0xFF7FC3E1);
          values[(int)KnownExtendedColors.Vegan] = unchecked((int)0xFF8EC298);
          values[(int)KnownExtendedColors.WateryBlue] = unchecked((int)0xFFD2E0DF);
          values[(int)KnownExtendedColors.Xylophone] = unchecked((int)0xFF996244);
          values[(int)KnownExtendedColors.Yucca] = unchecked((int)0xFF73978F);
          values[(int)KnownExtendedColors.ZebraGrass] = unchecked((int)0xFFA1A187);
        */


        #region Implicit Operators

        [Test]
        public void ImplicitOperatorExtendedColors()
        {
            Color c = Color.FromArgb(unchecked((int)0xFFA1A187));
            ExtendedColors ec = ExtendedColors.FromArgb(c.ToArgb());

            Assert.AreEqual(unchecked((int)0xFFA1A187), ec.ToArgb());
        }

        [Test]
        public void ImplicitOperatorColors()
        {
            Color c = Color.FromArgb(unchecked((int)0xFF996244));
            ExtendedColors ec = c;

            Assert.AreEqual(unchecked((int)0xFF996244), ec.ToArgb());
        }

        #endregion

        #region Properties

        [Test]
        public void IsEmptyTest()
        {
            ExtendedColors ec = new ExtendedColors();
            Assert.AreEqual(true, ec.IsEmpty);

            ExtendedColors ec2 = ExtendedColors.FromArgb(unchecked((int)0xFF795484));
            Assert.AreEqual(false, ec2.IsEmpty);
        }

        [Test]
        public void IsNamedColorTest()
        {
            ExtendedColors ec = ExtendedColors.FromName("VodoYellow");
            Assert.AreEqual(true, ec.IsNamedColor);

            ExtendedColors ec2 = ExtendedColors.FromArgb(unchecked((int)0xFF795482));
            Assert.AreEqual(false, ec2.IsNamedColor);
        }

        [Test]
        public void IsKnownColorTest()
        {
            ExtendedColors ec = ExtendedColors.FromName("MareaBaja"); 
            Assert.AreEqual(true, ec.IsKnownColor);

            ExtendedColors ec2 = ExtendedColors.FromArgb(unchecked((int)0xFF795482));
            Assert.AreEqual(false, ec2.IsKnownColor);
        }

        [Test]
        public void NameTest()
        {
            ExtendedColors ec = ExtendedColors.FromName("MareaBaja"); 
            Assert.AreEqual("MareaBaja", ec.Name);
        }

        [Test]
        public void RedTest()
        {
            ExtendedColors ec = ExtendedColors.FromName("MareaBaja"); 
            Assert.AreEqual(46, ec.R);
        }

        [Test]
        public void GRNTest()
        {
            ExtendedColors ec = ExtendedColors.FromName("MareaBaja"); 
            Assert.AreEqual(84, ec.G);
        }

        [Test]
        public void BLUTest()
        {
            ExtendedColors ec = ExtendedColors.FromName("MareaBaja"); 
            Assert.AreEqual(100, ec.B);
        }

        [Test]
        public void ALPATest()
        {
            ExtendedColors ec = ExtendedColors.FromName("MareaBaja"); 
            Assert.AreEqual(255, ec.A);
        }

        #endregion

        #region Public Methods

        [Test]
        public void FromKnownColorEnumTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja); 
            Assert.AreEqual(unchecked((int)0xFF2E5464), ec.ToArgb());
        }

        [Test]
        public void FromNameEnumTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja); 
            Assert.AreEqual(unchecked((int)0xFF2E5464), ec.ToArgb());
        }

        [Test]
        public void GetBrightnessTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja); 
            Color c = Color.FromArgb(unchecked((int)0xFF2E5464));
            Assert.AreEqual(Math.Round(c.GetBrightness(), MidpointRounding.AwayFromZero), Math.Round(ec.GetBrightness(), MidpointRounding.AwayFromZero));
        }

        [Test]
        public void GetHueTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja); 
            Color c = Color.FromArgb(unchecked((int)0xFF2E5464));
            Assert.AreEqual(Math.Round(c.GetHue(),MidpointRounding.AwayFromZero), Math.Round(ec.GetHue(),MidpointRounding.AwayFromZero));
        }

        [Test]
        public void GetSaturationTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja); 
            Color c = Color.FromArgb(unchecked((int)0xFF2E5464));
            Assert.AreEqual(Math.Round(c.GetSaturation(), MidpointRounding.AwayFromZero), Math.Round(ec.GetSaturation(), MidpointRounding.AwayFromZero));
        }


        [Test]
        public void ToArgbTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja);           
            Assert.AreEqual(unchecked((int)0xFF2E5464), ec.ToArgb());
        }


        [Test]
        public void ToKnownColorTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja);          
            Assert.AreEqual(unchecked(KnownExtendedColors.MareaBaja), ec.ToKnownColor());
        }

        [Test]
        public void ToStringTest()
        {
            ExtendedColors ec = ExtendedColors.FromKnownColor(KnownExtendedColors.MareaBaja);           
            Assert.AreEqual("ExtendedColors [MareaBaja]", ec.ToString());
        }

        //[Test]
        //public void ParseToColorTest()
        //{
        //    ExtendedColors ec = ExtendedColors.ParseToColor(unchecked((int)0xFF2E5464));          
        //    Assert.AreEqual(unchecked((int)0xFF2E5464), ec.ToArgb());
        //}

        [Test]
        public void FromArgbATest()
        {
            ExtendedColors ec = ExtendedColors.FromArgb(unchecked((int)0xFF2E5464));          
            Assert.AreEqual(unchecked((int)0xFF2E5464), ec.ToArgb());
        }

        [Test]
        public void FromArgbBTest()
        {
            ExtendedColors ec = ExtendedColors.FromArgb(46,84,100);            
            Assert.AreEqual(unchecked((int)0xFF2E5464), ec.ToArgb());
        }

        [Test]
        public void FromArgbCTest()
        {
            ExtendedColors ec = ExtendedColors.FromArgb(255,46, 84, 100);           
            Assert.AreEqual(unchecked((int)0xFF2E5464), ec.ToArgb());
        }

        [Test]
        public void FromArgbDTest()
        {
            ExtendedColors ec = ExtendedColors.FromArgb(90, ExtendedColors.MareaBaja);           
            Assert.AreEqual(unchecked((int)0x5A2E5464), ec.ToArgb());
            
        }

        [Test]
        public void EqualEqualTest()
        {
            ExtendedColors ec = ExtendedColors.MareaBaja;  
            ExtendedColors ecc = ExtendedColors.MareaBaja;        
            Assert.AreEqual(ecc, ec);

            ExtendedColors ec2 = ExtendedColors.MareaBaja;   
            ExtendedColors ecc2 = ExtendedColors.MarchWind;
            Assert.AreNotEqual(ecc2, ec2);
        }

        [Test]
        public void NotEqualTest()
        {
            ExtendedColors ec = ExtendedColors.MareaBaja;   
            ExtendedColors ecc = ExtendedColors.MarchWind;
            Assert.AreNotEqual(ecc, ec);

            ExtendedColors ec2 = ExtendedColors.MareaBaja;   
            ExtendedColors ecc2 = ExtendedColors.MareaBaja;
            Assert.AreEqual(false, ecc2 != ec2);
        }

        [Test]
        public void EqualsTest()
        {
            ExtendedColors ec = ExtendedColors.MareaBaja;   
            ExtendedColors ecc = ExtendedColors.MareaBaja;
            Assert.AreEqual(true, ec.Equals(ecc));
 
            ExtendedColors ecc2 = ExtendedColors.MarchWind;
            Assert.AreEqual(false, ec.Equals(ecc2));
        }

        #endregion
    }
}

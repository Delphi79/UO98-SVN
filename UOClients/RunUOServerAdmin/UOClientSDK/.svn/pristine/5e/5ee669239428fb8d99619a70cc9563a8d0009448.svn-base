using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UoClientSDK;
using UoClientSDK.Network;
using UoClientSDK.Network.ClientPackets;

namespace UOClientSDKTestProject
{
    /// <summary>
    ///This is a test class for CreateCharacterTest and is intended
    ///to contain all CreateCharacterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CreateCharacterTest
    {
        /// <summary>
        ///A test for CreateCharacter.GetBytes()
        ///</summary>
        [TestMethod()]
        public void CreateCharacterGetBytesTest()
        {
            ClientVersion version = ClientVersion.vMAX;

            SkillValuePair[] skills = new SkillValuePair[] {
                    new SkillValuePair() { Skill=(Skill)25, Value=50},
                    new SkillValuePair() { Skill=(Skill)26, Value=50},
                    new SkillValuePair() { Skill=(Skill)0, Value=0}};
            CharStats stats = new CharStats() { Strength = 50, Dexterity = 24, Intelligence = 1 };
            RaceAndGender raceandgender = new RaceAndGender(version, RaceAndGender.RaceType.Human, RaceAndGender.GenderType.Female);

            Hue pantscolor = (Hue)999;
            BeardStyle beardstyle = (BeardStyle)33;

            CreateCharacter target = new CreateCharacter(version, "David", ClientFlag.LBR, (Profession)1, raceandgender, stats, skills, 0, 0, 0, (BeardStyle)33, 0, 0, 0, IPAddress.None, 0, pantscolor);

            int expectedPacketLen = 104;

            PacketWriter packet = target.GetWriter();
            Assert.AreEqual(expectedPacketLen, packet.Length);
            Assert.AreEqual(skills[2].Skill, target.StartSkills[2].Skill);
            Assert.AreEqual(pantscolor, target.PantsColor);
            Assert.AreEqual(beardstyle, target.FacialHairStyle);
        }
    }
}

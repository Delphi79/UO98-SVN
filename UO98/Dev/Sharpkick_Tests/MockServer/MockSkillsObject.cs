using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick;

namespace Sharpkick_Tests
{
    partial class MockCore
    {
        private ISkillsObject _SkillsObject = new MockSkillsObject();

        public ISkillsObject SkillsObject { get { return _SkillsObject; } }

        class MockSkillsObject : ISkillsObject
        {
            #region ISkillsObject Members

            public int SkillCount
            {
                get { return 48; }
            }

            public SkillObjectStruct Skill(int skillnum)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}

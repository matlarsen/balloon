using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balloon {
    public interface Action {
        void DoAction();
        void StopAction();
    }
}

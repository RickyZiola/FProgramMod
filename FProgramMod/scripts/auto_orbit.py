import pythonnet
import System.Reflection
import System
import sys
import os
from clr_loader import get_coreclr
from pythonnet import set_runtime

rt = get_coreclr()
set_runtime(rt)

import clr
sys.path.append(os.path.join(os.getcwd(), "bin/Debug/net6.0"))

clr.AddReference("SFS_Flight_Program_API")
clr.AddReference("UnityEngine")
import UnityEngine
from SFS_Flight_Program_API import Telemetry
UnityEngine.Debug.Log(Telemetry.GetShipPosition())
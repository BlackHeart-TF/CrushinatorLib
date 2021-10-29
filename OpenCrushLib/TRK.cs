using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibOpenCrush
{
    public class TRK
    {
        public uint version ;
        public string truckName ;
        public string truckMake ;
        public string truckModel ;
        public string truckClass ;
        public uint truckCost ;
        public uint truckModelYear ;
        public float truckLength ;
        public float truckWidth ;
        public float truckHeight ;
        public float truckWheelbase ;
        public float truckFrontTrack ;
        public float truckRearTrack ;
        public float truckAcceleration ;
        public float truckTopSpeed ;
        public float truckHandling ;
        public uint truckQuickClass ;
        public string truckModelBaseName ;
        public string tireModelBaseName ;
        public string axleModelName ;
        public string shockTextureName ;
        public string barTextureName ;
        public Point3D axlebarOffset = new Point3D(0.000000f,0.000000f,0.000000f);
        public Point3D driveshaftPos = new Point3D(0.000000f,0.000000f,0.000000f);
        public driverHead driverHead;
        public eng eng = new eng();
        public whipAntenna whipAntenna;
        public string teamRequirement ;
        public axle faxle;
        public axle raxle;
        public xm xm;

        public fuel fuel;

        public float dry_weight ;
        public string ixx, iyy, izz ;
        public Point3D refArea = new Point3D(125.510002f,172.610001f,55.410000f);
        public float CL ;
        public float CD ;
        public float CY ;
        public uint drive_type ;
        public Point3D cgModifier = new Point3D(0.000000f,0.000000f,0.000000f);
        public float steering_scaler ;
        public float brakeBalance ;
        public float max_brake_force_pct ;
        public string clp, cmq, cnr ;
        public uint frontSuspType ;
        public uint rearSuspType ;
        public uint frontSuspSpringType ;
        public uint rearSuspSpringType ;


        private Stream file;
        private Metadata metadata;
        public TRK(Metadata metadata)
        {
            this.metadata = metadata;
            this.file = metadata.FileStream;

            ParseTRK();
        }

        private void ParseTRK()
        {
            try                                                     // Attempt to read colortable (768 bytes, Adobe color table used for 256-color raw files)
            {
                this.file.Position = 0;
                StreamReader file = new StreamReader(this.file);
                var trk = file.ReadToEnd().Replace("\r", "");
                var lines = trk.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int line = 0;
                var trkType = typeof(TRK);
                
                while (line < lines.Length)
                {
                    var member = trkType.GetField(lines[line]);
                    if(member != null)
                    switch (member.FieldType.Name)
                    {
                        case "UInt32":
                            line++;
                            member.SetValue(this, uint.Parse(lines[line]));
                            break;
                        case "String":
                            line++;
                            member.SetValue(this, lines[line]);
                            break;
                        case "Single":
                            line++;
                            member.SetValue(this, float.Parse(lines[line]));
                            break;
                        case "Point3D":
                            line++;
                            var split = lines[line].Split(',');
                            if (split.Length != 3)
                                goto default;
                            member.SetValue(this, new Point3D(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2])));
                            break;
                        default:
                            throw new Exception("Error parsing TRK at: " + lines[line]);
                            //break;
                    }
                    line++;
                }

            }
            catch (IOException e)
            {
                string debugStr = "ERROR: Failed to load TRK: " + metadata.Name + "!";
                Console.WriteLine(debugStr);
                Console.WriteLine(e);
            }
        }
    }
    public struct Point3D
    {
        public float X;
        public float Y;
        public float Z;

        public Point3D(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
    public class fuel
    {
        public float weight ;
    }
    public class whipAntenna
    {
        public Point3D bPos = new Point3D(2.366830f,2.695370f,-7.029810f);
    }
    public class driverHead
    {
        public Point3D initBPos = new Point3D(-1.220000f,3.370000f,0.280000f);
    }
    public class xm
    {
        public uint maxGear ;
        public List<float> gear_ratio;
        public List<float> gearInertia;
        public float final_drive ;
        public float axle_ratio ;
        public float pct_loss ;
        public float xfrLowRatio ;
        public uint type ;

        public uint maxGear_Auto;
        internal List<float> gear_ratio_Auto;
        internal List<float> gearInertia_Auto;
        internal float final_drive_Auto;
        internal float axle_ratio_Auto;
        internal float pct_loss_Auto;
        internal float xfrLowRatio_Auto;
        internal uint type_Auto;

        public xm()
        {
            gear_ratio = new List<float>();
            gearInertia = new List<float>();
            gear_ratio_Auto = new List<float>();
            gearInertia_Auto = new List<float>();
        }
    }
    public class axle
    {
        public tire rtire;
        public tire ltire;
        public float maxangle ;
        public float maxcompr ;
        public float spring_rate ;
        public float torque_pct ;
        public float axleBias ;
        public uint slipDiffType ;
        public axle()
        {
            rtire = new tire();
            ltire = new tire();
        }
    }
    public class tire
    {
        public Point3D static_bpos = new Point3D(+2.290000f,-0.700000f,+5.360000f);
        public float spring_arm ;
        public float CF ;
        public Point3D shockTopOffset;
        public Point3D shockBottomOffset;
    }
    public class eng
    {
        public float maxHP ;
        public float maxHPRPM ;
        public float maxTorque ;
        public float maxTorqueRPM ;
        public float redline ;
        public float redlineTimer ;
        public uint torqueTableCount ;
        public float upshift_rpm ;
        public float dnshift_rpm ;
        public float friction_cf ;
        public float fuel_consum ;
        public uint type ;
        public uint aspiration ;
        public float displacement ;
        internal float volEffiency;
        internal float idleSpeed;
        internal uint fuelType;
        internal List<float> torqueTable = new List<float>();
    }
}

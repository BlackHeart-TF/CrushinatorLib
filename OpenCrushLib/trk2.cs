using System;
using System.Collections.Generic;
using System.IO;



// Truck file version history:
//
// 1 .. MTM
// 2 .. MTM2
// 3 AB Added version control for MC3
// 4 AB Added make, model, class, cost, driverHead/whipAntenna pos, tire CFs, 
//            cgModifier, max_brake_force_pct, clp, cmq, cnr, susp/spring 
//            types, RGBs
// 5 AB Added truckModelYear, eng.type, eng.aspiration, eng.displacement, 
//            truckLength, truckWidth, truckHeight, truckWheelbase, 
//            truckFrontTrack, truckRearTrack, xm.xfrLowRatio, xm.type, 
//            faxle.slipDiffType, raxle.slipDiffType, stock part list
// 6 AB Added truckQuickClass, RGB color names
// 7 CB Added teamRequirement
// 8 Jeep Evolution, adds auto transmission ratios
// 9 4x4 Evo R, Seprate f/r wheel models
// 10 Author Information, Control Arm Model, Max manifold pressure
namespace LibOpenCrush
{
    public class TRK2
    {
        float GRAVITY_MOD = 1f;
        readonly string[] Makes = new string[] { "Custom", "Chevrolet", "Dodge", "Ford", "GMC", "Infiniti", "Mitsubishi", "Jeep", "Lexus", "Nissan", "Toyota" };
        readonly string[] gearName = { "", "Park", "Reverse", "Neutral", "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th" };
        const int TRKFILE_VERSION = 10;
        const int MAX_TRUCK_LIGHTS = 10;



        public uint version = 0;
        public string truckName;
        public string authorName;
        public string authorDesc;
        public string truckDesc;
        public string truckMake;
        public string truckModel;
        public string truckClass;
        public uint truckQuickClass;
        public uint truckCost;
        public uint truckModelYear;
        public float truckLength;
        public float truckWidth;
        public float truckHeight;
        public float truckWheelbase;
        public float truckFrontTrack;
        public float truckRearTrack;
        public float truckAcceleration;
        public float truckTopSpeed;
        public float truckHandling;
        public uint truckQuickClassl;
        public string truckModelBaseName;
        public string tireModelBaseName;
        public string tireModelBaseName_rear;
        public string axleModelName;
        public string shockTextureName;
        public string suspModelBaseName;
        public string barTextureName;
        public Point3D axlebarOffset;
        public Point3D driveshaftPos;
        public driverHead driverHead;
        public eng eng = new eng();
        public whipAntenna whipAntenna;
        public string teamRequirement;
        public axle faxle;
        public axle raxle;
        public xm xm;

        public fuel fuel;

        public float dry_weight;
        public float ixx, iyy, izz;
        public Point3D refArea;
        public float CL;
        public float CD;
        public float CY;
        public uint drive_type;
        public Point3D cgModifier;
        public float steering_scaler;
        public float brakeBalance;
        public float max_brake_force_pct;
        public float clp, cmq, cnr;
        public uint frontSuspType;
        public uint rearSuspType;
        public uint frontSuspSpringType;
        public uint rearSuspSpringType;
        public string panelName;
        private string waveFile;
        private string waveFile1;
        private string waveFile2;
        private uint numLights;

        private Point3D GetPoint3D(StreamReader file)
        {
            var line = file.ReadLine();
            var floats = Array.ConvertAll(line.Split(','), x => float.Parse(x));
            if (floats.Length != 3)
                throw new InvalidCastException(line);
            return new Point3D(floats[0], floats[1], floats[2]);
        }
        private float GetFloat(StreamReader file)
        {
            var line = file.ReadLine();
            float result = 0f;
            if (float.TryParse(line.Trim(),out result))
            {
                return result;
            }
            else
                throw new InvalidCastException(line);
        }
        private uint GetUInt(StreamReader file)
        {
            var line = file.ReadLine();
            uint result = 0;
            if (uint.TryParse(line.Trim(), out result))
            {
                return result;
            }
            else
                throw new InvalidCastException(line);
        }
        private void GetFloatPair(StreamReader file, out float Float1, out float Float2)
        {
            var line = file.ReadLine();
            var pair = line.Split(',');
            if (pair.Length != 2) throw new InvalidCastException(line);

            if (!float.TryParse(pair[0].Trim(), out Float1))
                throw new InvalidCastException(line);
            if (!float.TryParse(pair[1].Trim(), out Float2))
                throw new InvalidCastException(line);
        }
        private void GetUIntPair(StreamReader file, out uint UInt1, out uint UInt2)
        {
            var line = file.ReadLine();
            var pair = line.Split(',');
            if (pair.Length != 2) throw new InvalidCastException(line);

            if (!uint.TryParse(pair[0].Trim(), out UInt1))
                throw new InvalidCastException(line);
            if (!uint.TryParse(pair[1].Trim(), out UInt2))
                throw new InvalidCastException(line);
        }
        private void GetFloatTrip(StreamReader file, out float Float1, out float Float2, out float Float3)
        {
            var line = file.ReadLine();
            var pair = line.Split(',');
            if (pair.Length != 3) throw new InvalidCastException(line);

            if (!float.TryParse(pair[0].Trim(), out Float1))
                throw new InvalidCastException(line);
            if (!float.TryParse(pair[1].Trim(), out Float2))
                throw new InvalidCastException(line);
            if (!float.TryParse(pair[2].Trim(), out Float3))
                throw new InvalidCastException(line);
        }
        private void GetUIntTrip(StreamReader file, out uint UInt1, out uint UInt2, out uint UInt3)
        {
            var line = file.ReadLine();
            var pair = line.Split(',');
            if (pair.Length != 3) throw new InvalidCastException(line);

            if (!uint.TryParse(pair[0].Trim(), out UInt1))
                throw new InvalidCastException(line);
            if (!uint.TryParse(pair[1].Trim(), out UInt2))
                throw new InvalidCastException(line);
            if (!uint.TryParse(pair[2].Trim(), out UInt3))
                throw new InvalidCastException(line);
        }
        private void Init()
        {
            faxle = new axle();
            raxle = new axle();
            xm = new xm();
            
        }
        public TRK2(Metadata File)
        {
            Init();
            File.FileStream.Position = 0;
            var file = new StreamReader(File.FileStream);

            // Read in the structure of the .TRK file

            // Check for Evo version 3 or higher
            if (char.ToUpper((char)file.Peek()) == 'V')
            {
                file.ReadLine();                                                        // file.scanf("version\n");
                var str = file.ReadLine();// file.scanf("%d\n", version);                                              // Read TRK version
                uint.TryParse(str, out version);
            }

            // file.scanf("truckName\n"); (IN MTM1/2 only!
            var truckVersion = file.ReadLine();

            // Check if an older file format
            if (version == 0)
            {
                if (truckVersion == "2") version = 2;                                            // Version 2, has the number on another line
                else version = 1;                                                          // Version 1, did not have a version number
            }


            // Check for invalid version.  (This would mean an old .exe)
            if (version > TRKFILE_VERSION)
            {
                System.Console.WriteLine("Invalid truck version: %d, current version is %d.  (You probably have an old .exe)", truckVersion, TRKFILE_VERSION);
                return;
            }
            // truckName = file.ReadLine();
            truckName = file.ReadLine();

            //if (TruckName.Length >= truckName.Length) {
            //	GTFO("Truck name longer than %d characters: %s", sizeof(truckName) - 1, text);
            //}
            //strcpy(truckName, text);                                                             // Commit truck name to memory


            if (version >= 10)
            {
                file.ReadLine();                                                            // file.scanf("authorName\n");	
                authorName = file.ReadLine();
                //if (strlen(text) >= sizeof(authorName))
                //{                                       // Check the length of the author name so we can't exceed the buffer.
                //    GTFO("Author name longer than %d characters: %s", sizeof(authorName) - 1, text);
                //}
                //strcpy(authorName, text);                                                       // Commit author name to memory

                file.ReadLine();                                                            // file.scanf("authorDesc\n");
                authorDesc = file.ReadLine();
                //if (strlen(text) >= sizeof(authorDesc))
                //{                                       // Check the length of the author name so we can't exceed the buffer.
                //    GTFO("Author description longer than %d characters: %s", sizeof(authorDesc) - 1, text);
                //}
                //strcpy(authorDesc, text);                                                       // Commit author description to memory

                file.ReadLine();                                                            // file.scanf("truckDesc\n");
                truckDesc = file.ReadLine();
                //if (strlen(text) >= sizeof(truckDesc))
                //{
                //    GTFO("Author name longer than %d characters: %s", sizeof(truckDesc) - 1, text);
                //}
                //strcpy(truckDesc, text);
            }
            else
            {
                authorName = "N/A";
                authorDesc = "N/A";
                truckDesc = "N/A";
            }

            if (version >= 4)
            {
                file.ReadLine();                                                            // file.scanf("truckMake\n");
                var Make = file.ReadLine();

                file.ReadLine();                                                              // file.scanf("truckModel\n");
                truckModel = file.ReadLine();

                file.ReadLine();                                                               // file.scanf("truckClass\n");
                truckClass = file.ReadLine();

                file.ReadLine();                                                               // file.scanf("truckCost\n");
                truckCost = GetUInt(file);

                // Force all vehicles to custom if they don't match the following. (Kludge, manufacturers should be automatic not hard coded)

                if (Array.IndexOf(Makes, Make) > -1)
                    truckMake = Make;
                else
                    truckMake = "Custom";                                               // If none of the above match do this.

            }
            else if (version <= 2)
            {                                                       // Add support for MTM 2 and earlier
                truckClass = "Monster Truck";
                truckCost = 250000;
            }

            if (version >= 5)
            {
                file.ReadLine();                                //		file.scanf("truckModelYear\n");
                truckModelYear = GetUInt(file);
                file.ReadLine();                                //		file.scanf("truckLength\n");
                truckLength = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckWidth\n");
                truckWidth = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckHeight\n");
                truckHeight = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckWheelbase\n");
                truckWheelbase = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckFrontTrack\n");
                truckFrontTrack = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckRearTrack\n");
                truckRearTrack = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckAcceleration\n");
                truckAcceleration = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckTopSpeed\n");
                truckTopSpeed = GetFloat(file);
                file.ReadLine();                                //		file.scanf("truckHandling\n");
                truckHandling = GetFloat(file);
            }
            else if (version <= 2)
            {                           // MTM 2 and eariler defaults
                truckModelYear = 2015;
                truckLength = 12.0f;
                truckWidth = 5.0f;
                truckHeight = 6.0f;
                truckWheelbase = 10.0f;
                truckFrontTrack = 6.0f;
                truckRearTrack = 6.0f;
                truckAcceleration = 1.0f;
                truckTopSpeed = 1.0f;
                truckHandling = 1.0f;
            }

            if (version >= 6)
            {
                file.ReadLine();                                // file.scanf("truckQuickClass\n");
                truckQuickClass = GetUInt(file);
            }
            else if (version <= 2)
            {
                truckQuickClass = 4;                                // Assign all monster trucks to MTM class.
            }
            else
            {
                truckQuickClass = 5;                                // Assign all others to custom
            }

            // Read visual data

            file.ReadLine();									// file.scanf("truckModelBaseName\n");
            truckModelBaseName = file.ReadLine();

            if (version <= 8)
            {                               // Versions prior to 8 only have one model option
                file.ReadLine();                                // file.scanf("tireModelBaseName\n");
                tireModelBaseName = file.ReadLine();
                tireModelBaseName_rear = tireModelBaseName; // Load the same model front and rear
            }

            if (version >= 9)
            {                               // Versions after 9 support different models per axle
                file.ReadLine();                                // file.scanf("tireModelBaseName_front\n");
                tireModelBaseName = file.ReadLine();
                file.ReadLine();                                // file.scanf("tireModelBaseName_rear\n");
                tireModelBaseName_rear = file.ReadLine();
            }

            if (version >= 2)
            {
                file.ReadLine();                                // file.scanf("axleModelName\n");
                axleModelName = file.ReadLine();
                file.ReadLine();                                // file.scanf("shockTextureName\n");
                shockTextureName = file.ReadLine();
                file.ReadLine();                                // file.scanf("barTextureName\n");
                barTextureName = file.ReadLine();

                if (version >= 10)
                {                           // Version 10 adds suspension models!
                    file.ReadLine();                            // file.scanf("suspModelBaseName\n");
                    suspModelBaseName = file.ReadLine();

                    file.ReadLine();                            // file.scanf("shockTopOffset\n");
                    faxle.rtire.shockTopOffset = GetPoint3D(file);
                    faxle.ltire.shockTopOffset = GetPoint3D(file);
                    raxle.rtire.shockTopOffset = GetPoint3D(file);
                    raxle.ltire.shockTopOffset = GetPoint3D(file);

                    file.ReadLine();                            // file.scanf("shockBottomOffset\n");
                                                                //file.scanf("%f,%f,%f\n",&shockBottomOffset.x,&shockBottomOffset.y,&shockBottomOffset.z);
                    faxle.rtire.shockBottomOffset = GetPoint3D(file);
                    faxle.ltire.shockBottomOffset = GetPoint3D(file);
                    raxle.rtire.shockBottomOffset = GetPoint3D(file);
                    raxle.ltire.shockBottomOffset = GetPoint3D(file);
                }
                else
                {
                    suspModelBaseName = "NewSuspTest";          // Default to this model, NEVER GONNA GIVE YOU UP ALWAYS GONNA AUTOSIZE!
                }
                file.ReadLine();                                // file.scanf("axlebarOffset\n");
                axlebarOffset = GetPoint3D(file);
                file.ReadLine();                                // file.scanf("driveshaftPos\n");
                driveshaftPos = GetPoint3D(file);

                if (string.Compare(axleModelName, "NULL.BIN") == 0) axleModelName = null;
                if (string.Compare(shockTextureName, "NULL.RAW") == 0) shockTextureName = null;
                if (string.Compare(barTextureName, "NULL.RAW") == 0) barTextureName = null;
            }
            else
            {   // Defaults for MTM1
                axleModelName = "AXLE.BIN";
                shockTextureName = "SHOCK.RAW";
                barTextureName = "AXLEBAR.RAW";
            }

            // !KLUDGE! - Handle Evo 2 monster truck
            //  WHY IN THE HECK IS THIS HARD CODED, FIX THE TRK! FFS, IT WAS ALREADY CODED IN.
            if (string.Compare(Path.GetFileName(File.Name).ToUpper(), "AMT.TRK") == 0)
            {
                shockTextureName = "shock2.tif";
                barTextureName = "axlebar2.tif";
                axlebarOffset = new Point3D(1.3f, 0.1f, 0.55f);
                driveshaftPos = new Point3D(0.0f, 0.6f, 0.55f);
            }

            if (version >= 4)
            {
                driverHead = new driverHead();
                file.ReadLine();                            // file.scanf("driverHead.initBPos\n");
                driverHead.initBPos = GetPoint3D(file);
                whipAntenna = new whipAntenna();
                file.ReadLine();                            // file.scanf("whipAntenna.bPos\n");
                whipAntenna.bPos = GetPoint3D(file);
            }

            if (version >= 7)
            {
                file.ReadLine();                            // file.scanf("teamRequirement\n");
                teamRequirement = file.ReadLine();
                //int team = gRace.findTeamByName(teamreq);  // IS this needed?
                //if (team < 0)
                //{
                //    teamRequirement = 0;
                //}
                //else
                //{
                //    teamRequirement = team;
                //}
            }

            // Read tire data
            if (version >= 3)
            {
                file.ReadLine();                            // file.scanf("faxle.rtire.static_bpos\n");
                faxle.rtire.static_bpos = GetPoint3D(file);
                file.ReadLine();                            // file.scanf("faxle.ltire.static_bpos\n");
                faxle.ltire.static_bpos = GetPoint3D(file);
                file.ReadLine();                            // file.scanf("raxle.rtire.static_bpos\n");
                raxle.rtire.static_bpos = GetPoint3D(file);
                file.ReadLine();                            // file.scanf("raxle.ltire.static_bpos\n");
                raxle.ltire.static_bpos = GetPoint3D(file);
            }
            else
            {                                           // MTM 2 and lower
                file.ReadLine();
                faxle.rtire.static_bpos.X = GetFloat(file);
                file.ReadLine();
                faxle.ltire.static_bpos.X = GetFloat(file);
                file.ReadLine();
                raxle.rtire.static_bpos.X = GetFloat(file);
                file.ReadLine();
                raxle.ltire.static_bpos.X = GetFloat(file);
                file.ReadLine();
                faxle.rtire.static_bpos.Y = GetFloat(file);
                file.ReadLine();
                faxle.ltire.static_bpos.Y = GetFloat(file);
                file.ReadLine();
                raxle.rtire.static_bpos.Y = GetFloat(file);
                file.ReadLine();
                raxle.ltire.static_bpos.Y = GetFloat(file);
                file.ReadLine();                            //		file.scanf("faxle.rtire.static_bpos.z\n");
                faxle.rtire.static_bpos.Z = GetFloat(file);
                file.ReadLine();                            //		file.scanf("faxle.ltire.static_bpos.z\n");
                faxle.ltire.static_bpos.Z = GetFloat(file);
                file.ReadLine();                            //		file.scanf("raxle.rtire.static_bpos.z\n");
                raxle.rtire.static_bpos.Z = GetFloat(file);
                file.ReadLine();                            //		file.scanf("raxle.ltire.static_bpos.z\n");
                raxle.ltire.static_bpos.Z = GetFloat(file);
            }

            // Read additional tire data
            if (version >= 3)
            {       // Spring arm is not used even slightly in Evo. :( Version 10 removes this reference
                if (version <= 9)
                {
                    file.ReadLine();                            // file.scanf("faxle.rtire.spring_arm\n");
                    faxle.rtire.spring_arm = GetFloat(file);
                    file.ReadLine();                            // file.scanf("faxle.ltire.spring_arm\n");
                    faxle.ltire.spring_arm = GetFloat(file);
                    file.ReadLine();                            // file.scanf("raxle.rtire.spring_arm\n");
                    raxle.rtire.spring_arm = GetFloat(file);
                    file.ReadLine();                            // file.scanf("raxle.ltire.spring_arm\n");
                    raxle.ltire.spring_arm = GetFloat(file);
                }
                if (version >= 4)
                {                       // Tire traction
                    file.ReadLine();                        // file.scanf("faxle.rtire.CF\n");
                    faxle.rtire.CF = GetFloat(file);
                    file.ReadLine();                        // file.scanf("faxle.ltire.CF\n");
                    faxle.ltire.CF = GetFloat(file);
                    file.ReadLine();                        // file.scanf("raxle.rtire.CF\n");
                    raxle.rtire.CF = GetFloat(file);
                    file.ReadLine();                        // file.scanf("raxle.ltire.CF\n");
                    raxle.ltire.CF = GetFloat(file);
                }
                else
                {
                    faxle.ltire.CF = 2.0f;                      // MTM 2 default traction settings
                    faxle.rtire.CF = 2.0f;
                    raxle.ltire.CF = 2.2f;
                    raxle.rtire.CF = 2.2f;
                }


                // Read axle data
                file.ReadLine();                            // file.scanf("faxle.maxangle\n");
                faxle.maxangle = GetFloat(file);
                file.ReadLine();                            // file.scanf("raxle.maxangle\n");
                raxle.maxangle = GetFloat(file);
                file.ReadLine();                            // file.scanf("faxle.maxcompr\n");
                faxle.maxcompr = GetFloat(file);
                file.ReadLine();                            // file.scanf("raxle.maxcompr\n");
                raxle.maxcompr = GetFloat(file);
                file.ReadLine();                            // file.scanf("faxle.spring_rate\n");
                faxle.spring_rate = GetFloat(file);
                file.ReadLine();                            // file.scanf("raxle.spring_rate\n");
                raxle.spring_rate = GetFloat(file);
                file.ReadLine();                            // file.scanf("faxle.torque_pct\n");
                faxle.torque_pct = GetFloat(file);
                file.ReadLine();                            // file.scanf("raxle.torque_pct\n");
                raxle.torque_pct = GetFloat(file);
                file.ReadLine();                            // file.scanf("faxle.axleBias\n");
                faxle.axleBias = GetFloat(file);
                file.ReadLine();                            // file.scanf("raxle.axleBias\n");
                raxle.axleBias = GetFloat(file);

                if (version >= 5)
                {
                    file.ReadLine();                        // file.scanf("faxle.slipDiffType\n");
                    faxle.slipDiffType = GetUInt(file);
                    file.ReadLine();                        // file.scanf("raxle.slipDiffType\n");
                    raxle.slipDiffType = GetUInt(file);
                }

                // Read transmission data

                if (version <= 9)
                {
                    file.ReadLine();                        // file.scanf("xm.maxGear\n");
                    xm.maxGear = GetUInt(file);
                    file.ReadLine();                        // file.scanf("xm.gear_ratio[]\n");
                    for (int i = 1; i <= xm.maxGear; i++)
                    {
                        xm.gear_ratio.Add(GetFloat(file));
                    }
                    file.ReadLine();                        // file.scanf("xm.gearInertia[]\n");
                    for (int i = 1; i <= xm.maxGear; i++)
                    {
                        xm.gearInertia.Add(GetFloat(file));
                    }

                    file.ReadLine();                        // file.scanf("xm.final_drive\n");
                    xm.final_drive = GetFloat(file);
                    file.ReadLine();                        // file.scanf("xm.axle_ratio\n");
                    xm.axle_ratio = GetFloat(file);
                    file.ReadLine();                        // file.scanf("xm.pct_loss\n");
                    xm.pct_loss = GetFloat(file);

                    if (version >= 5)
                    {
                        file.ReadLine();                    // file.scanf("xm.xfrLowRatio\n");
                        xm.xfrLowRatio = GetFloat(file);
                        file.ReadLine();                    // file.scanf("xm.type\n");
                        xm.type = GetUInt(file);
                    }
                    // Version <= 7 backwards compatibility
                    xm.maxGear_Auto = xm.maxGear;
                    xm.gear_ratio_Auto = xm.gear_ratio;
                    xm.gearInertia_Auto = xm.gearInertia;


                    xm.final_drive_Auto = xm.final_drive;
                    xm.axle_ratio_Auto = xm.axle_ratio;
                    xm.pct_loss_Auto = xm.pct_loss;
                    xm.xfrLowRatio_Auto = xm.xfrLowRatio;
                    xm.type_Auto = xm.type;
                }
                else if (version >= 10)
                {                                   // EvoR Patch 11 and up
                    file.ReadLine();                        // file.scanf("xm.maxGear\n");
                    GetUIntPair(file, out xm.maxGear, out xm.maxGear_Auto);
                    file.ReadLine();                        // file.scanf("xm.gear_ratio[]\n");
                    for (int i = 1; i <= xm.maxGear; i++)
                    {
                        float f1;
                        float f2;
                        GetFloatPair(file, out f1, out f2);
                        xm.gear_ratio.Add(f1);
                        xm.gear_ratio_Auto.Add(f2);
                    }

                    file.ReadLine();                        // file.scanf("xm.gearInertia[]\n");
                    for (int i = 1; i <= xm.maxGear; i++)
                    {
                        float f1;
                        float f2;
                        GetFloatPair(file, out f1, out f2);
                        xm.gearInertia.Add(f1);
                        xm.gearInertia_Auto.Add(f2);
                    }

                    file.ReadLine();                        // file.scanf("xm.final_drive\n");
                    GetFloatPair(file, out xm.final_drive, out xm.final_drive_Auto);
                    file.ReadLine();                        // file.scanf("xm.axle_ratio\n");
                    GetFloatPair(file, out xm.axle_ratio, out xm.axle_ratio_Auto);
                    file.ReadLine();                        // file.scanf("xm.pct_loss\n");
                    GetFloatPair(file, out xm.pct_loss, out xm.pct_loss_Auto);
                    file.ReadLine();                        // file.scanf("xm.xfrLowRatio\n");
                    GetFloatPair(file, out xm.xfrLowRatio, out xm.xfrLowRatio_Auto);
                    file.ReadLine();                        // file.scanf("xm.type\n");
                    GetUIntPair(file, out xm.type, out xm.type_Auto);
                }

                // Jeep Evo 2 Compatibility, adds auto tranny ratios
                if (version >= 8 && version <= 9)
                {
                    file.ReadLine();                        // file.scanf("trans->maxGear\n");
                    xm.maxGear_Auto = GetUInt(file);
                    file.ReadLine();                        // file.scanf("trans->gear_ratio[] - ");
                    for (int i = 1; i <= xm.maxGear_Auto; i++)
                    {
                        xm.gear_ratio_Auto.Add(GetFloat(file));
                    }

                    file.ReadLine();                        // file.scanf("trans->gearInertia[]\n");
                    for (int i = 1; i <= xm.maxGear_Auto; i++)
                    {
                        xm.gearInertia_Auto.Add(GetFloat(file));
                    }

                    file.ReadLine();                        // file.scanf("trans->final_drive\n");
                    xm.final_drive_Auto = GetFloat(file);
                    file.ReadLine();                        // file.scanf("trans->axle_ratio\n");
                    xm.axle_ratio_Auto = GetFloat(file);
                    file.ReadLine();                        // file.scanf("trans->pct_loss\n");
                    xm.pct_loss_Auto = GetFloat(file);
                    file.ReadLine();                        // file.scanf("trans->xfrLowRatio\n");
                    xm.xfrLowRatio_Auto = GetFloat(file);
                    file.ReadLine();                        // file.scanf("trans->type\n");
                    xm.type_Auto = GetUInt(file);
                }

                // Read engine data
                file.ReadLine();                            // file.scanf("eng.maxHP\n");
                eng.maxHP = GetFloat(file);

                if (version <= 9)
                {
                    file.ReadLine();                        // file.scanf("eng.maxHPRPM\n");
                    eng.maxHPRPM = GetFloat(file);
                }
                file.ReadLine();                            // file.scanf("eng.maxTorque\n");
                eng.maxTorque = GetFloat(file);

                if (version <= 9)
                {
                    file.ReadLine();                        // file.scanf("eng.maxTorqueRPM\n");
                    eng.maxTorqueRPM = GetFloat(file);
                }
                else if (version >= 10)
                {
                    file.ReadLine();                        // file.scanf("eng.VE\n");
                    eng.volEffiency = GetFloat(file);

                }

                if (version <= 8 && version >= 3)
                {
                    eng.idleSpeed = 800.0f;                     // Default speed is 800RPM
                    eng.fuelType = 0;                           // Default fuel type to gas
                }
                else if (version >= 9)
                {
                    file.ReadLine();                        // file.scanf("eng.idleSpeed\n");	Sets engine idle speed
                    eng.idleSpeed = GetFloat(file);
                    file.ReadLine();                        // file.scanf("eng.fuelType\n");  Used for engine sounds
                    eng.fuelType = GetUInt(file);
                }
                else if (version <= 2)
                {                   // MTM trucks
                    eng.idleSpeed = 1100.0f;                    // Default speed is 1100RPM
                    eng.fuelType = 2;                           // Default fuel type to alchy
                }

                file.ReadLine();                            // file.scanf("eng.redline\n");
                eng.redline = GetFloat(file);
                file.ReadLine();                            // file.scanf("eng.redlineTimer\n");
                eng.redlineTimer = GetFloat(file);

                if (version >= 4 && version <= 9)
                {   // Values for now on will be auto generated
                    file.ReadLine();                        // file.scanf("eng.torqueTableCount\n");
                    eng.torqueTableCount = GetUInt(file);
                    file.ReadLine();                        // file.scanf("eng.torqueTable[]\n");
                    eng.torqueTable = new List<float>();
                    for (int i = 0; i < eng.torqueTableCount; i++)
                    {
                        eng.torqueTable.Add(GetFloat(file));
                    }
                }
                else
                {                                       // Generate for Evo birthday, WHOOPS MISSED THAT ONE LULZ
                    eng.torqueTableCount = (uint)((eng.redline + 499.0) / 500.0) + 1;
                    float devMax = 2;
                    float devMod = devMax / eng.torqueTableCount;
                    float maxUnAdjTrq = 0;
                    for (int i = 0; i <= eng.torqueTableCount; i++)
                    {
                        float deviation = (eng.volEffiency * -2) + (devMod * i);
                        int rpm = i * 500;
                        float unAdjTrq = 1.0f / ((float)Math.Sqrt(2.0f * Math.PI) * ((float)Math.Exp(deviation * deviation) / 2.0f));
                        eng.torqueTable[i] = unAdjTrq;
                        if (unAdjTrq > maxUnAdjTrq) maxUnAdjTrq = unAdjTrq;
                    }
                    float trqMod = eng.maxTorque / maxUnAdjTrq;
                    for (int i = 0; i <= eng.torqueTableCount; i++)
                    {
                        eng.torqueTable[i] *= trqMod;
                    }
                }

                //!! KLUDGE Temporary fix for the fact torque converters/clutches aren't simulated -Fuzzy
                // Overwrites the first 4 values

                int startTorqueTable = 3;   //Start at 90% of 2000 RPM on the torque table (Reason for 90% is so it doesn't accidently become the peak and f*** up stats
                for (int i = 1; i < startTorqueTable; i++)
                {
                    if (eng.torqueTable[i] < eng.torqueTable[startTorqueTable + 1])
                    {       // If you have more torque here than at 2000RPM leave it be.
                        eng.torqueTable[i] = eng.torqueTable[startTorqueTable + 1] * 0.9f;
                    }
                }

                file.ReadLine();                            // file.scanf("eng.upshift_rpm\n");
                eng.upshift_rpm = GetFloat(file);
                file.ReadLine();                            //		file.scanf("eng.dnshift_rpm\n");
                eng.dnshift_rpm = GetFloat(file);
                file.ReadLine();                            //		file.scanf("eng.friction_cf\n");
                eng.friction_cf = GetFloat(file);
                file.ReadLine();                            //		file.scanf("eng.fuel_consum\n");
                eng.fuel_consum = GetFloat(file);

                if (version >= 5)
                {
                    file.ReadLine();                        //			file.scanf("eng.type\n");
                    eng.type = GetUInt(file);
                    //			file.scanf("eng.aspiration\n");
                    file.ReadLine();
                    eng.aspiration = GetUInt(file);
                    //			file.scanf("eng.displacement\n");
                    file.ReadLine();
                    eng.displacement = GetFloat(file);
                }

                // Read fuel data

                //		file.scanf("fuel.weight\n");
                file.ReadLine();
                fuel.weight = GetFloat(file);

            }
            else if (version <= 2)
            {                       // MTM 2 and earier, force it to 1450FT/LBS, and make the tranny ratios the same.
                steering_scaler = 1.0f;

                xm.maxGear = 6;
                xm.gear_ratio[1] = 0.000000f;
                xm.gear_ratio[2] = -2.294000f;
                xm.gear_ratio[3] = 0.000000f;
                xm.gear_ratio[4] = 3.057000f;
                xm.gear_ratio[5] = 1.798000f;
                xm.gear_ratio[6] = 1.250000f;

                xm.gearInertia[1] = 0.0f;
                xm.gearInertia[2] = 1.98f;
                xm.gearInertia[3] = 0.0f;
                xm.gearInertia[4] = 2.98f;
                xm.gearInertia[5] = 1.9f;
                xm.gearInertia[6] = 0.98f;

                xm.final_drive = 7.5f;
                xm.axle_ratio = 1.0f;

                xm.maxGear_Auto = xm.maxGear;
                xm.gear_ratio_Auto = xm.gear_ratio;
                xm.gearInertia_Auto = xm.gearInertia;

                xm.final_drive_Auto = xm.final_drive;
                xm.axle_ratio_Auto = xm.axle_ratio;
                xm.pct_loss_Auto = xm.pct_loss;
                xm.xfrLowRatio_Auto = xm.xfrLowRatio;
                xm.type_Auto = xm.type;
                eng.idleSpeed = 1000.0f;
                eng.fuelType = 2;

                eng.torqueTableCount = (uint)((eng.redline + 499.0) / 500.0) + 1;
                eng.torqueTable = new List<float>();
                for (int i = 0; i < eng.torqueTableCount; i++)
                {
                    eng.torqueTable.Add(957);
                }
            }

            // Read scrape point data

            if (version >= 3)
            {
                //		file.scanf("sc[].pt\n");
                file.ReadLine();
            }

            for (int i = 1; i <= 12; i++)
            {
                if (version < 3)
                {
                    //			file.scanf("Scrape point %d body axis x,y,z\n",i);
                    file.ReadLine();
                }
                props->sc[i].pt = GetPoint3D(file);
            }

            // Read non-struct performance data

            if (version >= 3)
            {
                //		file.scanf("dry_weight\n");
                file.ReadLine();
                dry_weight = GetFloat(file);
                //@JO 4/23/01 - To get vehicles to handle I had to account for the increased gravity.
                dry_weight *= GRAVITY_MOD;
                //		file.scanf("ixx,iyy,izz\n");
                file.ReadLine();
                GetFloatTrip(file, out ixx, out iyy, out izz);
                //		file.scanf("refArea\n");
                file.ReadLine();
                refArea = GetPoint3D(file);
                //		file.scanf("CL\n");
                file.ReadLine();
                CL = GetFloat(file);
                //		file.scanf("CD\n");
                file.ReadLine();
                CD = GetFloat(file);
                //		file.scanf("CY\n");
                file.ReadLine();
                CY = GetFloat(file);
                //		file.scanf("drive_type\n");
                file.ReadLine();
                drive_type = GetUInt(file);
                //		file.scanf("cgModifier\n");
                file.ReadLine();
                if (version >= 4)
                {
                   cgModifier = GetPoint3D(file);
                }
                else
                {
                    cgModifier = new Point3D(0, 0, 0);
                    cgModifier.Y = GetFloat(file);
                }
                //		file.scanf("steering_scaler\n");
                file.ReadLine();
                steering_scaler = GetFloat(file);
                //		file.scanf("brakeBalance\n");
                file.ReadLine();
                brakeBalance = GetFloat(file);
            }

            if (version >= 4)
            {
                //		file.scanf("max_brake_force_pct\n");
                file.ReadLine();
                max_brake_force_pct = GetFloat(file);
                //		file.scanf("clp,cmq,cnr\n");
                file.ReadLine();
                GetFloatTrip(file, out clp, out cmq, out cnr);
                //@JO 3/01/01 - This change will apply to all platforms now.
                //		#ifdef PS2
                clp = cmq = -0.05f;
                //		#endif
                //		file.scanf("frontSuspType\n");
                file.ReadLine();
                frontSuspType = GetUInt(file);
                //		file.scanf("rearSuspType\n");
                file.ReadLine();
                rearSuspType = GetUInt(file);
                //		file.scanf("frontSuspSpringType\n");
                file.ReadLine();
                frontSuspSpringType = GetUInt(file);
                //		file.scanf("rearSuspSpringType\n");
                file.ReadLine();
                rearSuspSpringType = GetUInt(file);
            }

            // Read cockpit panel data

            if (file.Peek() == 'I')
            {
                file.ReadLine();
                panelName = file.ReadLine();
                if (string.Compare(panelName, "NULL") == 0) panelName = null;
            }
            else
            {
                panelName = null;
            }

            // Read wave file data

            if (file.Peek() == 'W')
            {
                file.ReadLine();
                waveFile = file.ReadLine();
                waveFile1 = file.ReadLine();
                waveFile2 = file.ReadLine();
            }
            else
            {
                waveFile = "default.wav";
                waveFile1 = "default.wav";
                waveFile2 = "default.wav";
            }

            // Read light data

            numLights = 0;
            memset(lightList, 0, sizeof(lightList));

            if (file.Peek() == 'N')
            {
                //		file.scanf("Number of Lights\n");
                file.ReadLine();
                numLights = GetUInt(file);
                if (numLights > MAX_TRUCK_LIGHTS)
                {
                    throw new ArgumentException(string.Format("Too many lights on %s!  Max = %d.", truckName, MAX_TRUCK_LIGHTS));
                }
                for (int j = 0; j < numLights; ++j)
                {
                    truckLightStruct l = lightList[j];
                    //			file.scanf("Light %d type\n", j);
                    file.ReadLine();
                    file.scanf("%d\n", l.type);
                    //			file.scanf("Light %d body axis pos x,y,z (ft), bitmap radius (ft)\n", j);
                    file.ReadLine();
                    file.scanf("%f,%f,%f,%f\n", l.initbpos.X, l.initbpos.Y, l.initbpos.Z, l.radius);
                    //			file.scanf("Light %d heading (rad), pitch (rad), heading spin speed (rad/sec)\n", j);
                    file.ReadLine();
                    file.scanf("%f,%f,%f\n", l.initHeading, l.initPitch, l.headingSpinSpeed);
                    //			file.scanf("Light %d cone: length (ft), base radius (ft), rim radius (ft), texture name\n", j);
                    file.ReadLine();
                    file.scanf("%f,%f,%f,%s\n", l.coneLength, l.coneBaseRadius, l.coneRimRadius, l.coneTexture.name);
                    //			file.scanf("Light %d source: bitmap name\n", j);
                    file.ReadLine();
                    file.scanf("%s\n", l.sourceTexturename);
                    //			file.scanf("Light %d ms on, ms off (0 if light doesn't blink)\n", j);
                    file.ReadLine();
                    file.scanf("%d,%d\n", &l.msOn, &l.msOff);

                    if (string.Compare(l.coneTexture.name, "NULL.RAW") == 0) l.coneTexture.name[0] = '\0';
                    if (string.Compare(l.sourceTexture.name, "NULL.RAW") == 0) l.sourceTexture.name[0] = '\0';
                }
                resetTruckLights(this);
            }
            else
            {
                defaultTruckLights(this);
            }

            //	// Read stock part data

            //	numStockParts = 0;
            //	memset(stockPartList,0,sizeof(stockPartList));

            //	if (truckVersion >= 5) {
            ////		file.scanf("numStockParts\n");
            //		file.ReadLine();
            //		numStockParts = GetUInt(file);
            ////		file.scanf("stockPartList[]\n");
            //		file.ReadLine();
            //		if (numStockParts > MAX_TRUCK_STOCK_PARTS) {
            //			GTFO("Too many stock parts for %s!  Max = %d.",truckName,MAX_TRUCK_STOCK_PARTS);
            //		}
            //		for (i = 0; i < numStockParts; i++) {
            //			text = file.ReadLine();

            //			// This section is for part workaround/backwards compatibility!
            //			if (strcmp(text, "aitlocker") == 0) {	text = "AirLockers";	}	// Kludge! TRI initially had this item listed as aitlockers, appears to be a typo/inside joke. Regardless it has to be supported.
            //			if (strcmp(text, "AitLockers") == 0) {	text = "AirLockers";	}	// Kludge! TRI initially had this item listed as aitlockers, appears to be a typo/inside joke. Regardless it has to be supported.
            //			if (strcmp(text, "Winch") == 0) {	text = "Winch1";	}	// Evo 1 backwards compatibility: Will equip any truck with this part with the weakest winch available.
            //			// End compatibility section.

            //			int part = gRace->findPartByCode(text);
            //			if (part < 0) {
            //				GTFO("Invalid stock part for %s!  Part = %s.",truckName,text);
            //			}
            //			stockPartList[i] = part;
            //		}
            //	} else if (truckVersion <= 2) {		// Add parts need for MTM2 vehicles
            //		numStockParts = 8;
            //		stockPartList[0] = gRace->findPartByCode("AirLockers");
            //		stockPartList[1] = gRace->findPartByCode("RearSteer");
            //		stockPartList[2] = gRace->findPartByCode("RaceTransferCase");
            //		stockPartList[3] = gRace->findPartByCode("BrakeBiasController");
            //		stockPartList[4] = gRace->findPartByCode("Racegearbox");
            //		stockPartList[5] = gRace->findPartByCode("RingGearSet");
            //		stockPartList[6] = gRace->findPartByCode("AirLockers");
            //		stockPartList[7] = gRace->findPartByCode("18RaceSprings");
            //	}

            //	// Read color data

            //	currentColor = 0;
            //	numColors = 0;
            //	memset(colorList,0,sizeof(colorList));

            //	if (truckVersion >= 3) {
            ////		file.scanf("numColors\n");
            //		file.ReadLine();
            //		numColors = GetUInt(file);
            ////		file.scanf("colorList[]\n");
            //		file.ReadLine();
            //		if (numColors > MAX_TRUCK_COLORS) {
            //			GTFO("Too many colors for %s!  Max = %d.",truckName,MAX_TRUCK_COLORS);
            //		}
            //		for (i = 0; i < numColors; i++) {
            //			truckColorStruct *c = &colorList[i];
            //			if (truckVersion >= 6) {
            //				file.scanf("%f,%f,%f,%f,%f,%f,%[^\n]\n",&c->dh,&c->ds,&c->dv,&c->r,&c->g,&c->b,text);
            //				if (strlen(text) >= sizeof(c->name)) {
            //					GTFO("Color name longer than %d characters: %s",sizeof(c->name) - 1,text);
            //				}
            //				if (stricmp(text,"NULL") != 0) {
            //					strcpy(c->name,text);
            //				}
            //			} else if (truckVersion >= 4) {
            //				file.scanf("%f,%f,%f,%f,%f,%f\n",&c->dh,&c->ds,&c->dv,&c->r,&c->g,&c->b);
            //			} else {
            //				file.scanf("%f,%f,%f\n",&c->dh,&c->ds,&c->dv);
            //			}
            //		}
            //	}

            //	// Read animation data

            //	if (truckVersion >= 10) {
            //		int renderSusp = 2;
            //		numAnime = 0;
            //		memset(animeList,0,sizeof(animeList));
            //		char tempChar[255];
            //		char fragment[255];
            //		float testVar;

            //		//		file.scanf("Render Old Suspension\n");
            //		file.ReadLine();
            //		renderSusp = GetUInt(file);
            //		//		file.scanf("Number of Animated Parts\n");
            //		file.ReadLine();
            //		numAnime = GetUInt(file);
            //		if (numAnime > MAX_TRUCK_ANIME) {
            //			GTFO("Too many animated parts on %s!  Max = %d.",truckName,MAX_TRUCK_LIGHTS);
            //		}
            //		//		file.scanf("Start Animation Script\n");
            //		file.ReadLine();
            //		for (int j = 0 ; j < numAnime ; ++j) {
            //			truckAnimeStruct *a = &animeList[j];
            //			file.scanf("%255[^\n]\n", tempChar);

            //			//Error Checking
            //			if	(strncmp(tempChar,"CreateNull(", 11) == 0) {			// This just creates a part duplicate, and does nothing more! Mostly just for testing!
            //				a->animeCommand = 0;
            //				sscanf(tempChar, "%[^(](%[^,],%f,%f,%f", fragment, a->animePartName, &a->animeOriginPos.x, &a->animeOriginPos.y, &a->animeOriginPos.z);
            //			}
            //			else if	(strncmp(tempChar,"CreateShockTop(", 15) == 0) {
            //				a->animeCommand = 1;
            //				sscanf(tempChar, "%[^(](%[^,],%[^,],%i,%f,%f,%f,%f,%f,%f,%f,%f,%f", fragment, a->animePartName, a->animeParentName, &a->animeOriginType,&a->animeOriginPos.x, &a->animeOriginPos.y, &a->animeOriginPos.z, &a->animeEndPos.x, &a->animeEndPos.y, &a->animeEndPos.z, &a->animeScaleAxis.x, &a->animeScaleAxis.y, &a->animeScaleAxis.z);
            //			}
            //			else if	(strncmp(tempChar,"CreateShockBottom(", 18) == 0) {
            //				a->animeCommand = 2;
            //				sscanf(tempChar, "%[^(](%[^,],%[^,],%f,%f,%f,%f,%f,%f,%f,%f,%f", fragment, a->animePartName, a->animeParentName, &a->animeEndPos.x, &a->animeEndPos.y, &a->animeEndPos.z, &a->animeOriginPos.x, &a->animeOriginPos.y, &a->animeOriginPos.z, &a->animeScaleAxis.x, &a->animeScaleAxis.y, &a->animeScaleAxis.z);
            //			}
            //			else if	(strncmp(tempChar,"CreateSpring(", 13) == 0) {
            //				a->animeCommand = 3;
            //				sscanf(tempChar, "%[^(](%[^,],%[^,],%f,%f,%f,%f,%f,%f,%f,%f,%f", fragment, a->animePartName, a->animeParentName, &a->animeEndPos.x, &a->animeEndPos.y, &a->animeEndPos.z, &a->animeOriginPos.x, &a->animeOriginPos.y, &a->animeOriginPos.z, &a->animeScaleAxis.x, &a->animeScaleAxis.y, &a->animeScaleAxis.z);
            //			} else { 
            //				char debugText[512];
            //				sprintf (debugText, "Anime Error! Line %i\nCommand: %s\nString: %s", j, fragment, tempChar);
            //				GTFO(debugText); // ADD MORE SPECIFIC DEBUG INFO	
            //			}

            //		}


            //		file.scanf("%[^\n]\n", tempChar);
            //		file.scanf("%[^\n]\n", tempChar);
            //		file.scanf("%[^\n]\n", tempChar);
            //		file.scanf("%[^\n]\n", tempChar);
            //		file.scanf("%[^\n]\n", tempChar);
            //	}

            //	//////////////////////////////////////////////////////////////////////
            //	//
            //	// Set any remaining variables that need to be initialized or adjusted
            //	//
            //	//////////////////////////////////////////////////////////////////////

            //	// Let's figure out the freakin' body and tire model names once, right 
            //	// here, instead of all over the cache, render, and File Manager code.  
            //	// If this seems a little complex - it's to preserve full backwards 
            //	// compatibility while implementing the .SMF model format  @AB 1.25.00

            //	char modelName[32], modelExt[32], temp[32], *ext;

            //	// Get high detail body name

            //	strcpy(modelName,truckModelBaseName);
            //	ext = strchr(modelName,'.');
            //	if (ext) *ext = '\0';

            //	// generate the dirty model texture name
            //	sprintf(temp,"%s_D.TIF",modelName);	
            //	if (dosFileLength("ART",temp) != -1) {
            //		strcpy(dirtyTruck.name,temp);
            //	} else {
            //		dirtyTruck.name[0] = '\0';    // Patch 008: Makes legacy dirt optional
            //    }	

            //	// make the high detail model name
            //	strcat(modelName,".smf");

            //	if ((dosFileLength("models",modelName)) != -1) {
            //		strcpy(modelExt,".smf");
            //	} else {
            //		strcpy(modelExt,".bin");
            //	}

            //	strcpy(truckModelName[HIGH_DETAIL_TRUCK],truckModelBaseName);
            //	ext = strchr(truckModelName[HIGH_DETAIL_TRUCK],'.');
            //	if (ext) *ext = '\0';
            //	strcat(truckModelName[HIGH_DETAIL_TRUCK],modelExt);

            //	// Get lower detail body names, from next highest detail to lowest detail

            //	for (int j = MAX_TRUCK_DETAIL_LEVELS - 2; j >= 0; j--) {

            //		strcpy(temp,truckModelBaseName);
            //		ext = strchr(temp,'.');
            //		if (ext) *ext = '\0';

            //		strcpy(modelName,temp);
            //// @AB 5.9.2001 - The artists are using really long names now (actually it's 
            ////                also because we use the actual .S3D name now)
            ////		if (strlen(temp) <= 7) {
            //			sprintf(modelName,"%s%d%s",temp,j,modelExt);
            ////		} else {
            ////			sprintf(modelName + 7,"%d%s",j,modelExt);
            ////		}

            //		// If it exists use this name, otherwise set to higher detail body name

            //		if (dosFileLength("models",modelName) != -1) {
            //			strcpy(truckModelName[j],modelName);
            //		} else {
            //			strcpy(truckModelName[j],truckModelName[j + 1]);
            //		}

            //	}
            //	modelName = "NewSuspTest.smf";
            //	strcpy(suspModelName,modelName);

            //	// Get tire names

            //	if (truckVersion >= 2) {

            //		if (stricmp(props->staticName,"bigfoot.trk") == 0 || truckVersion == 2) {

            //			// Get MTM2 tire names

            //			sprintf(tireModelNameL[HIGH_DETAIL_TIRE]  ,"%s16l.bin",tireModelBaseName);
            //			sprintf(tireModelNameR[HIGH_DETAIL_TIRE]  ,"%s16r.bin",tireModelBaseName);
            //			sprintf(tireModelNameL[MEDIUM_DETAIL_TIRE],"%s10l.bin",tireModelBaseName);
            //			sprintf(tireModelNameR[MEDIUM_DETAIL_TIRE],"%s10r.bin",tireModelBaseName);
            //			sprintf(tireModelNameL[LOW_DETAIL_TIRE]   ,"%s08l.bin",tireModelBaseName);
            //			sprintf(tireModelNameR[LOW_DETAIL_TIRE]   ,"%s08r.bin",tireModelBaseName);

            //			sprintf(tireModelNameL_rear[HIGH_DETAIL_TIRE]  ,"%s16l%s",tireModelBaseName_rear,modelExt);
            //			sprintf(tireModelNameR_rear[HIGH_DETAIL_TIRE]  ,"%s16r%s",tireModelBaseName_rear,modelExt);
            //			sprintf(tireModelNameL_rear[MEDIUM_DETAIL_TIRE],"%s10l%s",tireModelBaseName_rear,modelExt);
            //			sprintf(tireModelNameR_rear[MEDIUM_DETAIL_TIRE],"%s10r%s",tireModelBaseName_rear,modelExt);
            //			sprintf(tireModelNameL_rear[LOW_DETAIL_TIRE]   ,"%s08l%s",tireModelBaseName_rear,modelExt);
            //			sprintf(tireModelNameR_rear[LOW_DETAIL_TIRE]   ,"%s08r%s",tireModelBaseName_rear,modelExt);
            //		} else {
            //			strcpy(modelName,tireModelBaseName);
            //			strcat(modelName,"16l.smf");

            //			if ((dosFileLength("models",modelName)) != -1) {
            //				strcpy(modelExt,".smf");

            //				// Get LOD left and right tire names

            //				sprintf(tireModelNameL[HIGH_DETAIL_TIRE]  ,"%s16l%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameR[HIGH_DETAIL_TIRE]  ,"%s16r%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameL[MEDIUM_DETAIL_TIRE],"%s12l%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameR[MEDIUM_DETAIL_TIRE],"%s12r%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameL[LOW_DETAIL_TIRE]   ,"%s08l%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameR[LOW_DETAIL_TIRE]   ,"%s08r%s",tireModelBaseName,modelExt);

            //				sprintf(tireModelNameL_rear[HIGH_DETAIL_TIRE]  ,"%s16l%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameR_rear[HIGH_DETAIL_TIRE]  ,"%s16r%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameL_rear[MEDIUM_DETAIL_TIRE],"%s12l%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameR_rear[MEDIUM_DETAIL_TIRE],"%s12r%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameL_rear[LOW_DETAIL_TIRE]   ,"%s08l%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameR_rear[LOW_DETAIL_TIRE]   ,"%s08r%s",tireModelBaseName_rear,modelExt);
            //			} else {
            //				strcpy(modelName,tireModelBaseName);
            //				strcat(modelName,"l.smf");

            //				if ((dosFileLength("models",modelName)) != -1) {
            //					strcpy(modelExt,".smf");
            //				} else {
            //					strcpy(modelExt,".bin");
            //				}

            //				// Get only left and right tire names

            //				sprintf(tireModelNameL[HIGH_DETAIL_TIRE]  ,"%sl%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameR[HIGH_DETAIL_TIRE]  ,"%sr%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameL[MEDIUM_DETAIL_TIRE],"%sl%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameR[MEDIUM_DETAIL_TIRE],"%sr%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameL[LOW_DETAIL_TIRE]   ,"%sl%s",tireModelBaseName,modelExt);
            //				sprintf(tireModelNameR[LOW_DETAIL_TIRE]   ,"%sr%s",tireModelBaseName,modelExt);

            //				sprintf(tireModelNameL_rear[HIGH_DETAIL_TIRE]  ,"%sl%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameR_rear[HIGH_DETAIL_TIRE]  ,"%sr%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameL_rear[MEDIUM_DETAIL_TIRE],"%sl%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameR_rear[MEDIUM_DETAIL_TIRE],"%sr%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameL_rear[LOW_DETAIL_TIRE]   ,"%sl%s",tireModelBaseName_rear,modelExt);
            //				sprintf(tireModelNameR_rear[LOW_DETAIL_TIRE]   ,"%sr%s",tireModelBaseName_rear,modelExt);
            //			}
            //		}

            //	} else {

            //		strcpy(tireModelNameL[HIGH_DETAIL_TIRE]  ,tireModelBaseName);
            //		strcpy(tireModelNameR[HIGH_DETAIL_TIRE]  ,tireModelBaseName);
            //		strcpy(tireModelNameL[MEDIUM_DETAIL_TIRE],tireModelBaseName);
            //		strcpy(tireModelNameR[MEDIUM_DETAIL_TIRE],tireModelBaseName);
            //		strcpy(tireModelNameL[LOW_DETAIL_TIRE]   ,tireModelBaseName);
            //		strcpy(tireModelNameR[LOW_DETAIL_TIRE]   ,tireModelBaseName);

            //		strcpy(tireModelNameL_rear[HIGH_DETAIL_TIRE]  ,tireModelBaseName_rear);
            //		strcpy(tireModelNameR_rear[HIGH_DETAIL_TIRE]  ,tireModelBaseName_rear);
            //		strcpy(tireModelNameL_rear[MEDIUM_DETAIL_TIRE],tireModelBaseName_rear);
            //		strcpy(tireModelNameR_rear[MEDIUM_DETAIL_TIRE],tireModelBaseName_rear);
            //		strcpy(tireModelNameL_rear[LOW_DETAIL_TIRE]   ,tireModelBaseName_rear);
            //		strcpy(tireModelNameR_rear[LOW_DETAIL_TIRE]   ,tireModelBaseName_rear);

            //	}

            //	// Check for dirty tire texture left

            ////	strcpy(temp,tireModelNameL[HIGH_DETAIL_TIRE]);
            ////	ext = strchr(temp,'.');
            ////	if (ext) *ext = '\0';
            ////	strcat(temp,"_D.TIF");
            ////	if (dosFileLength("ART",temp) != -1) {
            ////		strcpy(dirtyTireL.name,temp);
            ////	} else {
            ////		dirtyTireL.name[0] = '\0';
            ////	}
            ////	
            ////	// Check for dirty tire texture right
            ////
            ////	strcpy(temp,tireModelNameR[HIGH_DETAIL_TIRE]);
            ////	ext = strchr(temp,'.');
            ////	if (ext) *ext = '\0';
            ////	strcat(temp,"_D.TIF");
            ////	if (dosFileLength("ART",temp) != -1) {
            ////		strcpy(dirtyTireR.name,temp);
            ////	} else {
            ////		dirtyTireR.name[0] = '\0';
            ////	}


            //	// Set manufacturer dimensions (for on-screen stats purposes)

            //	if (truckWheelbase == 0.0f)  truckWheelbase  = faxle.rtire.static_bpos.z - raxle.rtire.static_bpos.z;
            //	if (truckFrontTrack == 0.0f) truckFrontTrack = faxle.rtire.static_bpos.x - faxle.ltire.static_bpos.x;
            //	if (truckRearTrack == 0.0f)  truckRearTrack  = raxle.rtire.static_bpos.x - raxle.ltire.static_bpos.x;

            //	// Set the visual wheel z positions

            //	faxle_rtire_static_bpos_z_visual = faxle.rtire.static_bpos.z;
            //	faxle_ltire_static_bpos_z_visual = faxle.ltire.static_bpos.z;
            //	raxle_rtire_static_bpos_z_visual = raxle.rtire.static_bpos.z;
            //	raxle_ltire_static_bpos_z_visual = raxle.ltire.static_bpos.z;
            //	faxle_rtire_static_bpos_y_visual = faxle.rtire.static_bpos.y;

            //	// Set the visual wheel track scalar

            //	visualTrackScalar = 1.05f;

            //	// Now, check the sim wheel z positions

            //	if (truckVersion <= 2) {

            //		float	wheelbaseDiff;

            //		wheelbaseDiff = (fabs(faxle.rtire.static_bpos.z) + 
            //				 fabs(raxle.rtire.static_bpos.z)) - 11.6;

            //		if (wheelbaseDiff > 0.0) {
            //			faxle.rtire.static_bpos.z -= wheelbaseDiff / 2.0;
            //			raxle.rtire.static_bpos.z += wheelbaseDiff / 2.0;
            //			if (faxle.rtire.static_bpos.z < 0.0) faxle.rtire.static_bpos.z = 0.0;
            //			if (raxle.rtire.static_bpos.z > 0.0) raxle.rtire.static_bpos.z = 0.0;
            //		}

            //		wheelbaseDiff = (fabs(faxle.ltire.static_bpos.z) + 
            //				 fabs(raxle.ltire.static_bpos.z)) - 11.6;

            //		if (wheelbaseDiff > 0.0) {
            //			faxle.ltire.static_bpos.z -= wheelbaseDiff / 2.0;
            //			raxle.ltire.static_bpos.z += wheelbaseDiff / 2.0;
            //			if (faxle.ltire.static_bpos.z < 0.0) faxle.ltire.static_bpos.z = 0.0;
            //			if (raxle.ltire.static_bpos.z > 0.0) raxle.ltire.static_bpos.z = 0.0;
            //		}

            //	}

            //	// Set axle positions based on wheel positions

            //	faxle.bpos.y = faxle.rtire.static_bpos.y;
            //	faxle.bpos.z = faxle.rtire.static_bpos.z;

            //	raxle.bpos.y = raxle.rtire.static_bpos.y;
            //	raxle.bpos.z = raxle.rtire.static_bpos.z;

            //	// Set CF init & orig values

            //	faxle.rtire.origCF = faxle.rtire.CF;
            //	faxle.rtire.initCF = faxle.rtire.CF;
            //	faxle.ltire.origCF = faxle.ltire.CF;
            //	faxle.ltire.initCF = faxle.ltire.CF;
            //	raxle.rtire.origCF = raxle.rtire.CF;
            //	raxle.rtire.initCF = raxle.rtire.CF;
            //	raxle.ltire.origCF = raxle.ltire.CF;
            //	raxle.ltire.initCF = raxle.ltire.CF;

            //	// Set spring rates

            ////	if (frontSuspType == eSuspTypeInd) {
            ////		faxle.spring_rate *= 0.5F;
            ////		faxle.damping_cf *= 0.5F;
            ////	}
            ////
            ////	if (rearSuspType == eSuspTypeInd) {
            ////		raxle.spring_rate *= 0.5F;
            ////		raxle.damping_cf *= 0.5F;
            ////	}

            //	// Set transmission variables

            //	xm.origFinal_drive = xm.final_drive;
            //	xm.finalRatio = xm.final_drive * xm.xfrLowRatio * xm.axle_ratio;
            //	if (truckVersion < 5 && xm.gear_ratio[8] == 0.0f) {
            //		// They don't have any transmission type info or a fifth gear, 
            //		// so we'll assume it's an automatic
            //		xm.type = eXmissionTypeAuto;
            //	}

            //	// Torque scalar to increase or reduce all vehicles power when read in

            //	for (i = 0; i < eng.torqueTableCount; i++) {
            ////		eng.torqueTable[i] *= TORQUE_BOOST;
            ////@JO 4/23/01 - To get vehicles to handle I had to account for the increased gravity.
            //		eng.torqueTable[i] *= TORQUE_BOOST * GRAVITY_MOD;
            //	}

            //	eng.maxHP *= TORQUE_BOOST;
            //	eng.maxTorque *= TORQUE_BOOST;

            //	// Set drive mode

            //	if (drive_type == TWO_WHEEL_DRIVE) {
            //		driveMode = TWO_WHEEL_DRIVE_MODE;
            //	} else {
            //		driveMode = FOUR_WHEEL_DRIVE_HIGH_MODE;
            //	}

            //	// Set driver head position to initial position since game starts in slew mode

            //	driverHead.bPos = driverHead.initBPos;

            //	// Turn on the lights, if appropriate for the weather

            //	headlightsOn = 0;
            //	if (gWeather->wouldLightsShowUp()) headlightsOn = 2;


        }
    }

    internal class truckLightStruct
    {
        public string type;
        public Point3D initbpos;
        internal object radius;
        internal object initHeading;
        internal object initPitch;
        internal object headingSpinSpeed;
        internal object coneLength;
        internal object coneBaseRadius;
        internal object coneRimRadius;
        internal object coneTexture;
        internal object sourceTexturename;
    }
}
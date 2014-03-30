using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Linq;

namespace TestScriptCS.Scripts
{
    //負荷軽減のために、キャラと乗り物の取得をキャッシュする
    public class Cacher : Script
    {
        //ロックオブジェクト　多分シングルスレッドだからいらないはずだけど一応
        private static System.Object lockObject = new System.Object();

        private static Ped[] pedList = new Ped[0];
        private static Vehicle[] vehicleList = new Vehicle[0];

        
        public static Ped[] GetAllPeds()
        {
            lock (lockObject)
            {
                try
                {
                    if (pedList.Length == 0) { return new Ped[0]; }
                    return pedList.Where(p => p != null).Where(p => p.Exists()).ToArray<Ped>();
                }
                catch (Exception e)
                {
                    //極稀に例外が出るのでその時はnice catchする
                    return new Ped[0]; 
                }
            }
        }

        public static Ped[] GetPeds(Vector3 Position,float length)
        {
            lock (lockObject)
            {
                try
                {
                    if (pedList.Length == 0) { return new Ped[0]; }
                    //リストから一定距離のものだけを返す
                    return pedList.Where(p => p != null).Where(p => p.Exists()).Where(p => Position.DistanceTo(p.Position) <= length).ToArray<Ped>();
                }
                catch
                {
                    return new Ped[0]; 
                }
            }
        }

        public static Vehicle[] GetAllVehicles()
        {
            lock (lockObject)
            {
                try
                {
                    if (vehicleList.Length == 0) { return new Vehicle[0]; }
                    return vehicleList.Where(v => v != null).Where(v => v.Exists()).ToArray<Vehicle>();
                }
                catch
                {
                    return new Vehicle[0]; 
                }
            }
        }

        public static Vehicle[] GetVehicles(Vector3 Position, float length)
        {
            lock (lockObject)
            {
                try
                {
                    if (vehicleList.Length == 0) { return new Vehicle[0]; }
                    //リストから一定距離のものだけを返す
                    return vehicleList.Where(v => v != null).Where(v => v.Exists()).Where(v => Position.DistanceTo(v.Position) <= length).ToArray<Vehicle>();
                }
                catch
                {
                    return new Vehicle[0]; 
                }
            }
        }


        public Cacher()
        {
            this.Tick += new EventHandler(this.CacherTick);
            Interval = 100;
        }

        //ここで読み込んで配列に突っ込む
        private void CacherTick(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                pedList = GTA.World.GetPeds(Player.Character.Position, 200.0f);
                vehicleList = GTA.World.GetVehicles(Player.Character.Position, 300.0f);
            }
        }
  
    }
}

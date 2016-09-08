using System.Data.Entity;
using System.Linq;

namespace Business_Logic
{
    public class tblSystemLogic : baseLogic
    {



        public static tblSystem getSystemValueByKey(string key)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.tblSystems.FirstOrDefault(c => c.strKey == key);
            }
            catch
            {
                return null;
            }
        }

        public static void updateSystemValueByKey(tblSystem c)
        {

            try
            {

                BusProjectEntities db = new BusProjectEntities();
                db.Entry<tblSystem>(c).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {
            }
        }
 
     
    }
}

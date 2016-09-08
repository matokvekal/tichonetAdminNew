using System;
using System.Data.SqlClient;
using System.Linq;
using Business_Logic.Entities;
using Business_Logic.Helpers;

namespace Business_Logic
{
   public class ManageLogic: baseLogic
    {

        public static void addEmailSent(tblEmailSent c)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.tblEmailSents.Add(c);
                db.SaveChanges();
            }
            catch
            {
            }
        }
        public static void deleteDuplicatedStudentToLines()
        {
            try
            {

                BusProjectEntities db = new BusProjectEntities();
      

                    System.Data.SqlClient.SqlConnection cn;
                    cn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BusProject"].ConnectionString);
                     string sql = "DELETE FROM  StudentsToLines WHERE id IN ( "
                              +   " SELECT MIN(id) FROM  dbo.StudentsToLines "
                              +   " GROUP BY StudentId,LineId,StationId,Direction,Date,sun,mon,tue,wed,thu,fri,sat "
                              +   "  HAVING count(*) > 1) ";

                    cn.Open();
                    SqlCommand com = new SqlCommand(sql, cn);
                    com.ExecuteScalar();


                 string sql2 = "update tblstudent set color=null where pk in(SELECT    s.pk"
                              +   " FROM         tblStudent AS s LEFT OUTER JOIN"
                              +   " StudentsToLines AS t ON s.pk = t.StudentId "
                              + "  WHERE     (t.StudentId IS NULL) AND (s.Color IS NOT NULL)) ";
                  SqlCommand com2 = new SqlCommand(sql2, cn);
                  com2.ExecuteScalar();


                    cn.Close();
         

            }
            catch (Exception ex)
            {//todo write to log
                throw ex;
            }

        }

    }
}

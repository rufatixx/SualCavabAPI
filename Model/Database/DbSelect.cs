using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SualCavabAPI.Model.Structs;
using MySql.Data.MySqlClient;
using System.Net.Mail;

namespace SualCavabAPI.Model.Database
{
    public class DbSelect
    {

        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public DbSelect(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }





        public List<User> logIn(string mail, string password)
        {


            List<User> user = new List<User>();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();


                using (MySqlCommand com = new MySqlCommand("select *,(select name from gender where genderId=a.genderId) as gender," +

                     "(select name from city where cityId=a.cityId)as city," +
                     "(select name from .profession where professionId=a.professionId)as profession" +
                     " from user a where  email=@login and passwd=SHA2(@pass,512) and isActive=1", connection))
                {
                    com.Parameters.AddWithValue("@login", mail);
                    com.Parameters.AddWithValue("@pass", password);

                    using (MySqlDataReader reader = com.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                User usr = new User();

                                usr.ID = Convert.ToInt32(reader["userID"]);
                                usr.name = reader["name"] != null ? reader["name"].ToString() : "";
                                usr.surname = reader["surname"] != null ? reader["surname"].ToString() : "";
                                usr.mail = reader["email"] != null ? reader["email"].ToString() : "";
                                usr.phone = reader["mobile"] != null ? reader["mobile"].ToString() : "";
                                usr.birthDate = !string.IsNullOrEmpty(reader["birthdate"].ToString()) ? DateTime.Parse(reader["birthdate"].ToString()) : DateTime.MinValue;
                                usr.gender = reader["gender"] != null ? reader["gender"].ToString() : "";
                                usr.city = reader["city"] != null ? reader["city"].ToString() : "";
                                usr.profession = reader["profession"] != null ? reader["profession"].ToString() : "";
                                usr.regDate = !string.IsNullOrEmpty(reader["cdate"].ToString()) ? DateTime.Parse(reader["cdate"].ToString()) : DateTime.MinValue;

                                user.Add(usr);


                            }
                            //  connection.Close();


                        }
                    }


                    com.Dispose();


                }
                connection.Dispose();
                connection.Close();
            }


            return user;





        }

        public List<PublicationsStruct> publications(string mail, string pass, int topicID)

        {
            List<PublicationsStruct> publicationList = new List<PublicationsStruct>();
            List<User> userList = new List<User>();
            string topicQuery = "";
            string personilizedReactions = "";
            if (!string.IsNullOrEmpty(mail) && !string.IsNullOrEmpty(pass))
            {

                userList = logIn(mail, pass);
                personilizedReactions = $", (select reaction from publication_reactions where userID=@userID and publicationID=a.id limit 1)as reaction";
            }


            if (topicID > 0)
            {
                topicQuery = $"and topicID={topicID}";
            }

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {


                connection.Open();

                using (MySqlCommand com = new MySqlCommand("select *,(select name from topics where topicId=a.topicID ) as topicName," +
                    "(select name from user where userid=a.userid)as userName," +
                    "(select surname from user where userid=a.userid)as userSurname," +
                    "(select count(distinct userID) from publication_views where  publicationId=a.ID)as views," +
                    "(select count(*) from publication_reactions where reaction=1 and publicationId=a.id)as reactionCount," +
                    "(select count(*) from publication_comments where publicationId=a.id)as commentCount" +
                    $"{personilizedReactions}" +

                     $" from publications a where isActive=1 {topicQuery} order by cdate desc", connection))
                {

                    if (userList.Count > 0)
                    {
                        com.Parameters.AddWithValue("@userID", userList[0].ID);
                    }






                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {

                            PublicationsStruct publication = new PublicationsStruct();
                            publication.id = Convert.ToInt32(reader["ID"]);
                            publication.publisher = $"{reader["userName"].ToString()} {reader["userSurname"].ToString()}";
                            publication.name = reader["name"].ToString();

                            publication.description = reader["description"].ToString();

                            if (userList.Count > 0)
                            {
                                try
                                {
                                    publication.reaction = Convert.ToInt32(reader["reaction"]);
                                }
                                catch (Exception)
                                {

                                    publication.reaction = 0;
                                }


                            }



                            publication.reactionCount = Convert.ToInt32(reader["reactionCount"]);
                            publication.commentCount = Convert.ToInt32(reader["commentCount"]);
                            publication.aTypeId = Convert.ToInt32(reader["aTypeId"]);

                            publication.topicId = Convert.ToInt32(reader["topicID"]);
                            publication.topicName = reader["topicName"].ToString();
                            publication.cDate = DateTime.Parse(reader["cdate"].ToString());
                            publication.views = Convert.ToInt32(reader["views"]);
                            //publication.photoUrl.Add(reader["photoUrl"].ToString());



                            publicationList.Add(publication);


                        }




                    }
                    //connection.Close();
                    //Сортировка платных реклам по пользователю

                    com.Dispose();
                }



                //// connection.Open();
                //using (MySqlCommand com = new MySqlCommand("select * ,(select httpUrl from media where announcementId=a.announcementId limit 1) as photoUrl,(select name from category where categoryId=a.categoryId ) as categoryName, " +
                //"(select name from announcement_type where aTypeId=a.aTypeId ) as aTypeName " +
                //$"from announcement a where isPaid=1 and isActive=1 {categoryQuery} and (announcementID =(select distinct announcementId from announcement_view where announcementID=a.announcementID and userId=@userID and DATE_FORMAT(cdate, '%Y-%m-%d')<DATE_FORMAT(now(), '%Y-%m-%d')) or announcementId not in (select distinct announcementId from announcement_view where userId=@userID))order by cdate desc", connection))
                //{

                //    com.Parameters.AddWithValue("@userID", userID);
                //    MySqlDataReader reader = com.ExecuteReader();
                //    if (reader.HasRows)
                //    {

                //        while (reader.Read())
                //        {

                //            Advertisement ads = new Advertisement();
                //            ads.id = Convert.ToInt32(reader["announcementId"]);
                //            ads.name = reader["name"].ToString();
                //            ads.description = reader["description"].ToString();
                //            ads.price = reader["price"].ToString();
                //            ads.aTypeId = Convert.ToInt32(reader["aTypeId"]);
                //            ads.aTypeName = reader["aTypeName"].ToString();
                //            ads.isPaid = Convert.ToInt32(reader["isPaid"]);
                //            ads.mediaTpId = Convert.ToInt32(reader["mediaTpId"]);
                //            ads.catId = Convert.ToInt32(reader["categoryId"]);
                //            ads.catName = reader["categoryName"].ToString();
                //            ads.cDate = DateTime.Parse(reader["cdate"].ToString());
                //            ads.photoUrl = new List<string>();
                //            ads.photoUrl.Add(reader["photoUrl"].ToString());



                //            adsList.Add(ads);


                //        }

                //    }
                //    com.Dispose();
                //}
                connection.Close();
                connection.Dispose();
            }



            return publicationList;

        }
        public List<CommentStruct> comments(int publicationID)

        {
            List<CommentStruct> commentList = new List<CommentStruct>();


            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {


                connection.Open();

                using (MySqlCommand com = new MySqlCommand("select * from publication_comments where publicationid = @pID", connection))
                {

                    com.Parameters.AddWithValue("@pID", publicationID);






                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            if (reader["ID"] != null &&
                                reader["publicationId"] != null &&
                                reader["userId"] != null &&
                                reader["comment"] != null &&
                                reader["cdate"] != null
                                )
                            {


                                CommentStruct comment = new CommentStruct();
                                comment.ID = Convert.ToInt32(reader["ID"]);
                                comment.publicationID = Convert.ToInt32(reader["publicationId"]);
                                comment.userID = Convert.ToInt32(reader["userId"]);

                                // comment.comment = reader["comment"] !=null ? reader["comment"].ToString():"";
                                comment.comment = reader["comment"].ToString();

                                comment.cDate = Convert.ToDateTime(reader["cdate"]);


                                commentList.Add(comment);
                            }

                        }




                    }
                    //connection.Close();
                    //Сортировка платных реклам по пользователю

                    com.Dispose();
                }




                connection.Close();
                connection.Dispose();
            }



            return commentList;

        }
        public List<TopicStruct> pTopics()

        {




            List<TopicStruct> aCatList = new List<TopicStruct>();



            MySqlConnection connection = new MySqlConnection(ConnectionString);


            connection.Open();

            MySqlCommand com = new MySqlCommand("select  * from topics", connection);
            MySqlDataReader reader = com.ExecuteReader();
            if (reader.HasRows)
            {


                while (reader.Read())
                {
                    TopicStruct aCat = new TopicStruct();
                    aCat.ID = Convert.ToInt32(reader["topicId"]);
                    aCat.name = reader["name"].ToString();
                    aCat.topicImage = reader["topicImgUrl"].ToString();
                    aCatList.Add(aCat);
                }
            }


            connection.Close();

            return aCatList;
        }
        public List<PublicationsStruct> topPublicactions(string mail, string pass)

        {

            List<PublicationsStruct> publicationList = new List<PublicationsStruct>();
            List<User> userList = new List<User>();

            string personilizedReactions = "";
            if (!string.IsNullOrEmpty(mail) && !string.IsNullOrEmpty(pass))
            {

                userList = logIn(mail, pass);
                personilizedReactions = $", (select reaction from publication_reactions where userID=@userID and publicationID=a.id limit 1)as reaction";
            }





            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();

                MySqlCommand com = new MySqlCommand(@$"SELECT *
,(select name from topics where topicId=a.topicID ) as topicName,
                    (select name from user where userid=a.userid)as userName,
                    (select surname from user where userid=a.userid)as userSurname,
                    (select count(distinct userID) from publication_views where publicationId=a.ID)as views,
                    (select count(*) from publication_reactions where reaction=1 and publicationId=a.id)as reactionCount,
                    (select count(*) from publication_comments where publicationId=a.id)as commentCount,

(select count(*) from sualcavab.publication_reactions  where publicationId = a.id and reaction=1)as interestingCount {personilizedReactions}  FROM sualcavab.publications a where isActive=1 order by interestingCount desc limit 10;", connection);
                if (userList.Count > 0)
                {
                    com.Parameters.AddWithValue("@userID", userList[0].ID);
                }

                MySqlDataReader reader = com.ExecuteReader();
                if (reader.HasRows)
                {


                    while (reader.Read())
                    {
                        PublicationsStruct publication = new PublicationsStruct();
                        publication.id = Convert.ToInt32(reader["ID"]);
                        publication.publisher = $"{reader["userName"]} {reader["userSurname"].ToString()}";
                        publication.name = reader["name"].ToString();
                        publication.description = reader["description"].ToString();

                        if (userList.Count > 0)
                        {
                            try
                            {
                                publication.reaction = Convert.ToInt32(reader["reaction"]);
                            }
                            catch (Exception)
                            {

                                publication.reaction = 0;
                            }


                        }



                        publication.reactionCount = Convert.ToInt32(reader["reactionCount"]);
                        publication.commentCount = Convert.ToInt32(reader["commentCount"]);
                        publication.aTypeId = Convert.ToInt32(reader["aTypeId"]);

                        publication.topicId = Convert.ToInt32(reader["topicID"]);
                        publication.topicName = reader["topicName"].ToString();
                        publication.cDate = DateTime.Parse(reader["cdate"].ToString());
                        publication.views = Convert.ToInt32(reader["views"]);
                        //publication.photoUrl.Add(reader["photoUrl"].ToString());



                        publicationList.Add(publication);

                    }
                }


                connection.Close();
            }



            return publicationList;
        }


        public StatusStruct IsValid(string emailaddress)
        {
            StatusStruct status = new StatusStruct();
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {


                try
                {
                    MailAddress m = new MailAddress(emailaddress);






                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand("select * from user where email=@mail", connection))
                    {



                        com.Parameters.AddWithValue("@mail", emailaddress);
                        MySqlDataReader reader = com.ExecuteReader();

                        bool except = reader.HasRows;
                        com.Dispose();
                        connection.Close();
                        if (except)
                        {

                            status.response = 2;
                            status.responseString = "user exist";
                        }
                        else
                        {
                            status.response = 0;
                        }

                    }

                }
                catch (Exception ex)
                {
                    status.response = 1;
                    status.responseString = ex.Message;

                }

            }
            return status;
        }
    }

}

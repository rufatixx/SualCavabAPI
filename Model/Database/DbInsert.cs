﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SualCavabAPI.Model.Structs;
using MySql.Data.MySqlClient;

namespace SualCavabAPI.Model.Database
{
    public class DbInsert
    {

        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public DbInsert(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }



        public List<StatusStruct> addReaction(string mail, string pass, int publicationID, int reaction)

        {
            List<StatusStruct> statusList = new List<StatusStruct>();
            StatusStruct status = new StatusStruct();
            if (!string.IsNullOrEmpty(mail) && !string.IsNullOrEmpty(pass) && publicationID > 0 && reaction >= 0)
            {
                try
                {



                    DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
                    List<User> userList = select.logIn(mail, pass);
                    int reactionID = 0;
                    bool excepts = false;
                    if (userList.Count > 0)
                    {


                        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                        {


                            connection.Open();


                            using (MySqlCommand com = new MySqlCommand("select * from publication_reactions where userID=@userID and publicationID=@pID", connection))
                            {


                                com.Parameters.AddWithValue("@pID", publicationID.ToString());
                                com.Parameters.AddWithValue("@userID", userList[0].ID.ToString());

                            

                                
                                using (MySqlDataReader reader = com.ExecuteReader())
                                {

                                    if (reader.HasRows)
                                    {
                                        excepts = true;

                                    }
                                }


                                com.Dispose();
                            }
                            if (excepts)
                            {
                                using (MySqlCommand com = new MySqlCommand("update publication_reactions set reaction=@reaction,publicationID=@pID,userID=@userID,cdate=now() where userID=@userID and publicationID=@pID", connection))
                                {

                                    com.Parameters.AddWithValue("@reaction", reaction.ToString());
                                    com.Parameters.AddWithValue("@pID", publicationID.ToString());
                                    com.Parameters.AddWithValue("@userID", userList[0].ID.ToString());


                                    com.ExecuteNonQuery();


                                    com.Dispose();
                                }
                            }
                            else
                            {
                                using (MySqlCommand com = new MySqlCommand("insert into publication_reactions (reaction,publicationID,userID,cdate) values (@reaction,@publicationID,@userID,now())", connection))
                                {

                                    com.Parameters.AddWithValue("@reaction", reaction.ToString());
                                    com.Parameters.AddWithValue("@publicationID", publicationID.ToString());
                                    com.Parameters.AddWithValue("@userID", userList[0].ID.ToString());


                                    com.ExecuteNonQuery();


                                    com.Dispose();
                                }
                            }



                            connection.Close();
                            connection.Dispose();
                        }

                        status.response = 0;
                    }
                    else
                    {
                        status.response = 2;
                        status.responseString = "User not found";
                    }

                    }
                catch (Exception ex)
                {

                    status.response = 1;
                    status.responseString = ex.Message;

                }

            
        }
            statusList.Add(status);
            return statusList;

        }
        public List<StatusStruct> addComment(string mail, string pass, int publicationID,int commentID, string comment)

        {
            List<StatusStruct> statusList = new List<StatusStruct>();
            StatusStruct status = new StatusStruct();
            if (!string.IsNullOrEmpty(mail) && !string.IsNullOrEmpty(pass) && publicationID > 0 && !string.IsNullOrEmpty(comment))
            {
                try
                {


                    List<PublicationsStruct> publicationList = new List<PublicationsStruct>();
                    DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
                    List<User> userList = select.logIn(mail, pass);
                    int reactionID = 0;

                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {


                        connection.Open();
                        string replyQuery = "";
                        string replyParameter = "";
                        if (commentID > 0) {
                            replyQuery = ",comment_id";
                            replyParameter = ",@commentID";
                        }

                     

                            using (MySqlCommand com = new MySqlCommand($"insert into publication_comments (comment,publicationID,userID,cdate{replyQuery}) values (@comment,@publicationID,@userID,now(){replyParameter})", connection))
                            {

                                com.Parameters.AddWithValue("@comment", comment.ToString());
                            if (commentID > 0)
                            {
                               
                                com.Parameters.AddWithValue("@commentID", commentID.ToString());

                            }
                            com.Parameters.AddWithValue("@publicationID", publicationID.ToString());
                                com.Parameters.AddWithValue("@userID", userList[0].ID.ToString());


                                com.ExecuteNonQuery();


                                com.Dispose();
                            }
                        

                        connection.Close();
                        connection.Dispose();
                    }

                    status.response = 0;

                }
                catch (Exception ex)
                {

                    status.response = 1;
                    status.responseString = ex.Message;

                }
            }
            statusList.Add(status);
            return statusList;

        }
        public List<StatusStruct> signUp(NewUser newUser)

        {
            List<StatusStruct> statusList = new List<StatusStruct>();
            StatusStruct status = new StatusStruct();
            if (!string.IsNullOrEmpty(newUser.mail) && !string.IsNullOrEmpty(newUser.pass) && !string.IsNullOrEmpty(newUser.name) && !string.IsNullOrEmpty(newUser.surname)&&newUser.professionID>0)
            {
                DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
                status = select.IsValid(newUser.mail);
                if (status.response == 0)
                {
                    try
                    {





                        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                        {


                            connection.Open();




                            using (MySqlCommand com = new MySqlCommand("insert into user (name,surname,email,passwd,professionID) values (@name,@surname,@email,SHA2(@passwd,512),@professionID)", connection))
                            {

                                com.Parameters.AddWithValue("@name", newUser.name.ToString());
                                com.Parameters.AddWithValue("@surname", newUser.surname.ToString());
                                com.Parameters.AddWithValue("@email", newUser.mail.ToString());
                                com.Parameters.AddWithValue("@passwd", newUser.pass.ToString());
                                com.Parameters.AddWithValue("@professionID", newUser.professionID);


                                com.ExecuteNonQuery();


                                com.Dispose();
                            }


                            connection.Close();
                            connection.Dispose();
                        }

                        status.response = 0;

                    }
                    catch (Exception ex)
                    {

                        status.response = 1;
                        status.responseString = ex.Message;

                    }
                }

            }
            else {
                status.response = 3;
                status.responseString = "Params error";
            
            }
            statusList.Add(status);
            return statusList;

        }

        public List<StatusStruct> newPost(NewPublication newPublication)

        {
            List<StatusStruct> statusList = new List<StatusStruct>();
            StatusStruct status = new StatusStruct();
            if (!string.IsNullOrEmpty(newPublication.mail) && !string.IsNullOrEmpty(newPublication.pass) && !string.IsNullOrEmpty(newPublication.name) && newPublication.topicID>0&& newPublication.backgroundImageID>0)
            {
                DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
                List<User> userList = select.logIn(newPublication.mail, newPublication.pass);
                if (userList.Count>0)
                {
                    try
                    {





                        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                        {


                            connection.Open();




                            using (MySqlCommand com = new MySqlCommand("inser into publications (name,topicID,userID) values (@name,@topicID,@userID)", connection))
                            {

                                com.Parameters.AddWithValue("@name", newPublication.name.ToString());
                                com.Parameters.AddWithValue("@topicID", newPublication.topicID);
                                com.Parameters.AddWithValue("@userID", userList[0].ID);
                              


                                com.ExecuteNonQuery();


                                com.Dispose();
                            }
                            using (MySqlCommand com = new MySqlCommand("inser into media (name,topicID,userID) values (@name,@topicID,@userID)", connection))
                            {

                                com.Parameters.AddWithValue("@name", newPublication.name.ToString());
                                com.Parameters.AddWithValue("@topicID", newPublication.topicID);
                                com.Parameters.AddWithValue("@userID", userList[0].ID);



                                com.ExecuteNonQuery();


                                com.Dispose();
                            }


                            connection.Close();
                            connection.Dispose();
                        }

                        status.response = 0;

                    }
                    catch (Exception ex)
                    {

                        status.response = 1;
                        status.responseString = ex.Message;

                    }
                }

            }
            else
            {
                status.response = 3;
                status.responseString = "Params error";

            }
            statusList.Add(status);
            return statusList;

        }


    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PropertyRentalManagementWebSite.Models;

namespace PropertyRentalManagementWebSite.Controllers
{
    public class MessagesController : Controller
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();

        // GET: Messages
        public ActionResult Index()
        {
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var currentUserId = currentUser?.UserId;
            string userRole = Session["UserRole"] as string;

            if (userRole != "Manager" && userRole != "Tenant")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            IQueryable<Message> messagesQuery = db.Messages.Where(a => false); ;

            if (userRole == "Manager")
            {
                // If the user is a manager, they can see all Messages or specific ones based on their needs
                messagesQuery = db.Messages.Include(a => a.User).Include(a => a.User1);
                // Additional logic for managers, if any
            }
            else if (userRole == "Tenant")
            {

                // If the user is a tenant, they only see appointments related to them
                currentUserId = currentUser.UserId;
                messagesQuery = db.Messages.Where(a => a.Receiver == currentUserId || a.Sender == currentUserId)
                                                   .Include(a => a.User).Include(a => a.User1);
                // Additional logic for tenants, if any
            }

            var messagesList = messagesQuery.ToList();
            ViewBag.CurrentUserId = currentUserId;
            return View(messagesList);
           
        }

        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Messages/Create
        public ActionResult Create(int? replyToSenderId = null)
        {
            // Find the UserId based on the UserName in the database
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            ViewBag.IsReply = false;
            if (currentUser == null)
            {
                // Handle the case where the user is not found - perhaps redirect to an error page or log out
                return RedirectToAction("Error");
            }

            var message = new Message
            {
                // If user is found, it is safe to use the Value property because user.UserId will not be null
                Sender = currentUser.UserId,
                 SendDate = DateTime.Now
            };
            if (currentUser.UserType.UserRole == "Manager")
            {
                var tenants = db.Users.Where(u => u.UserType.UserRole == "Tenant").ToList();
                ViewBag.Receiver = new SelectList(tenants, "UserId", "UserName");
            }
            else if (currentUser.UserType.UserRole == "Tenant")
            {
                var managers = db.Users.Where(u => u.UserType.UserRole == "Manager").ToList();
                ViewBag.Receiver = new SelectList(managers, "UserId", "UserName");
            }
            // Prepare the default status
            var defaultStatus = db.Statuses.FirstOrDefault(s => s.Description == "UnRead");
            if (defaultStatus != null)
            {
                message.StatusId = defaultStatus.StatusId; // Assign the default status ID for new messages
            }
            else
            {
                // Handle the case where the default status is not found
                // You may want to log this error or handle it as per your business logic
                ModelState.AddModelError("", "Default status not found.");
                return View(message);
            }
            if (replyToSenderId.HasValue)
            {
                var originalSender = db.Users.Find(replyToSenderId.Value);
                if (originalSender != null)
                {
                    ViewBag.IsReply = true;
                    ViewBag.OriginalSenderId = originalSender.UserId;
                    ViewBag.OriginalSenderName = originalSender.UserName;
                    message.Receiver = originalSender.UserId;
                }
                else
                {
                    // Handle error: original sender not found
                    ViewBag.IsReply = false;
                }
            }
            else
            {
                ViewBag.IsReply = false;
                // Populate the Receiver SelectList for non-reply messages
                ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName");
            }

            // Pass the message  object to the view, which has the Sender set
            return View(message);
            
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MessageId,Receiver,Sender,Message1,SendDate")] Message message)
        {
            // Get the current user again to ensure we're setting the current logged-in user as the sender
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (currentUser == null)
            {
                // Handle the case where the user is not found - perhaps redirect to an error page or log out
                return RedirectToAction("Error"); // Replace with actual error handling
            }
            if (ModelState.IsValid)
            {
                // Set the send date here
                message.SendDate = DateTime.Now;
                // Set the sender from the current logged-in user
                message.Sender = currentUser.UserId;
                // Ensure the StatusId is set for the new message
                  var defaultStatus = db.Statuses.FirstOrDefault(s => s.Description == "UnRead");
                    if (defaultStatus != null)
                    {
                        message.StatusId = defaultStatus.StatusId;
                    }
                    else
                    {
                        // Handle the case where the default status is not found
                        ModelState.AddModelError("", "Default status not found.");
                        return View(message);
                    }
                
                 db.Messages.Add(message);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Message created successfully.";
                
                return RedirectToAction("Index");
            }

            ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName", message.Receiver);
            //ViewBag.Sender = new SelectList(db.Users, "UserId", "UserName", appointment.Sender);
            return View(message);

        }

        // GET: Messages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName", message.Receiver);
            ViewBag.Sender = new SelectList(db.Users, "UserId", "UserName", message.Sender);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MessageId,Receiver,Sender,Message1,SendDate")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Receiver = new SelectList(db.Users, "UserId", "UserName", message.Receiver);
            ViewBag.Sender = new SelectList(db.Users, "UserId", "UserName", message.Sender);
            return View(message);
        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAsRead(int messageId)
        {
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Error");
            }

            var message = db.Messages.Include(m => m.Status).FirstOrDefault(m => m.MessageId == messageId);
            if (message == null || message.Receiver != currentUser.UserId)
            {
                return RedirectToAction("UnauthorizedAccess");
            }

            if (message.Status.Description == "UnRead")
            {
                var readStatus = db.Statuses.FirstOrDefault(s => s.Description == "Read");
                if (readStatus != null)
                {
                    message.StatusId = readStatus.StatusId;
                    try
                    {
                        db.Entry(message).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Log the error (uncomment ex variable name and add logging here)
                        TempData["ErrorMessage"] = "An error occurred while marking the message as read.";
                        return RedirectToAction("Index");
                    }

                    TempData["SuccessMessage"] = "Message has been marked as read.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Read status not found.";
                }
            }
            else
            {
                TempData["InfoMessage"] = "Message is already marked as read.";
            }

            return RedirectToAction("Index"); // Redirect back to the dashboard
        }

        public ActionResult CreateReply(int receiverId, int originalMessageId)
        {
            var currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (currentUser == null)
            {
                
                return RedirectToAction("Error");
            }

            var originalSender = db.Users.Find(receiverId);
            if (originalSender == null)
            {
             
                return RedirectToAction("Error");
            }

            // Retrieve the original message by ID
            var originalMessage = db.Messages.Find(originalMessageId);
            if (originalMessage == null)
            {
                // Handle error: original message not found
                return RedirectToAction("Error");
            }

            // Create a new message as a reply, pre-filling some fields
            var message = new Message
            {
                Receiver = receiverId, // Set the receiver to the sender of the original message
                Sender = currentUser.UserId, // The current user is the sender
                Message1 = $"Replying to your message: \"{originalMessage.Message1}\"", // Example of quoting the original message
                SendDate = DateTime.Now,
                // Here you would set the default status ID for "Unread"
            };

            ViewBag.IsReply = true; // To identify in the view that this is a reply
            ViewBag.OriginalSenderName = originalSender.UserName; // To show in the view
            ViewBag.OriginalMessageContent = originalMessage.Message1; // To display the original message content in the view if needed

            return View("Create", message); // Reuse the Create view for replies
        }


    }
}

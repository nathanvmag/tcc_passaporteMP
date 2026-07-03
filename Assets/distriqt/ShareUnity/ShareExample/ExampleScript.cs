using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

using distriqt.plugins.share;
using distriqt.plugins.share.events;
using distriqt.plugins.share.applications;
using distriqt.plugins.share.email;
using distriqt.plugins.share.sms;


namespace distriqt.example.share
{
    public class ExampleScript : MonoBehaviour
    {

        public GameObject stateTextObject;
        public GameObject logTextObject;

        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Button button6;
        public Button button7;
        public Button button8;

        private Text stateText;
        private Text logText;


        void Start()
        {
            if (stateTextObject != null)
            {
                stateText = stateTextObject.GetComponent<Text>();
            }
            if (logTextObject != null)
            {
                logText = logTextObject.GetComponent<Text>();
                logText.text = "";
            }


            Button btn;
            btn = button1.GetComponent<Button>();
            btn?.onClick.AddListener(button1_OnClick);
            btn = button2.GetComponent<Button>();
            btn?.onClick.AddListener(button2_OnClick);
            btn = button3.GetComponent<Button>();
            btn?.onClick.AddListener(button3_OnClick);
            btn = button4.GetComponent<Button>();
            btn?.onClick.AddListener(button4_OnClick);
            btn = button5.GetComponent<Button>();
            btn?.onClick.AddListener(button5_OnClick);
            btn = button6.GetComponent<Button>();
            btn?.onClick.AddListener(button6_OnClick);
            btn = button7.GetComponent<Button>();
            btn?.onClick.AddListener(button7_OnClick);
            btn = button8.GetComponent<Button>();
            btn?.onClick.AddListener(button8_OnClick);






            UpdateState(
                "Share.isSupported: " + Share.isSupported + "\n" +
                "Share.version: " + Share.Instance.Version()
            );

            //
            //  Check whether the plugin is supported on the current platform

            if (Share.isSupported)
            {
                Share.Instance.OnCancelled += Share_OnCancelled;
                Share.Instance.OnClosed += Share_OnClosed;
                Share.Instance.OnComplete += Share_OnComplete;
                Share.Instance.OnFailed += Share_OnFailed;


                Share.Instance.SMS.OnSMSSent += SMS_OnSMSSent;
                Share.Instance.SMS.OnSMSSentError += SMS_OnSMSSentError;
                Share.Instance.SMS.OnSMSCancelled += SMS_OnSMSCancelled;
                Share.Instance.SMS.OnSMSDelivered += SMS_OnSMSDelivered;
                Share.Instance.SMS.OnSMSNotDelivered += SMS_OnSMSNotDelivered;
                Share.Instance.SMS.OnSMSReceived += SMS_OnSMSReceived;
            }

        }

       

        private void button1_OnClick()
        {
            ShareUrl();
        }

        private void button2_OnClick()
        {
            ShareFile();
        }

        private void button3_OnClick()
        {
            ShowOpenIn();
        }

        private void button4_OnClick()
        {
            ShareScreenshot();
        }

        private void button5_OnClick()
        {
            LaunchApplication();
        }

        private void button6_OnClick()
        {
            SendEmail();
        }

        private void button7_OnClick()
        {
            SendEmailWithOptions();
        }

        private void button8_OnClick()
        {
            SendSMS();
        }



        void UpdateState( string state )
        {
            if (stateText != null)
            {
                stateText.text = state;
            }
        }

        void Log(string message)
        {
            if (logText != null)
            {
                logText.text = message + "\n" + logText.text;
            }
            Debug.Log(message);
        }







        private void Share_OnFailed(ShareEvent e)
        {
            Log("Share_OnFailed");
        }

        private void Share_OnComplete(ShareEvent e)
        {
            Log("Share_OnComplete:" + e.activityType);
        }

        private void Share_OnClosed(ShareEvent e)
        {
            Log("Share_OnClosed");
        }

        private void Share_OnCancelled(ShareEvent e)
        {
            Log("Share_OnCancelled");
        }



        //
        //  Share Url
        //

        private void ShareUrl()
        {
            if (Share.isSupported)
            {
                ShareOptions options = new ShareOptions();
                options.position = new Rect(100, 100, 200, 50);
                options.arrowDirection = ShareOptions.ARROWDIRECTION_ANY;

                Share.Instance.share("Check out this site", null, "https://distriqt.com", options );
            }
            else
            {
                Log("Share not supported");
            }
        }


        //
        //  Share File
        //

        private void ShareFile()
        {
            if (Share.isSupported)
            {
                string filePath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "image.png");
                if (!File.Exists(filePath))
                {
                    Log("File not found");
                    return;
                }

                Share.Instance.shareFile( filePath, "image.png", "image/png", null, true );
            }
            else
            {
                Log("Share not supported");
            }
        }

        

        //
        //  Show OpenIn
        //

        private void ShowOpenIn()
        {
            if (Share.isSupported)
            {
                string filePath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "image.png");
                if (!File.Exists(filePath))
                {
                    Log("File not found");
                    return;
                }

                Share.Instance.showOpenIn(filePath, "image.png", "image/png");
            }
            else
            {
                Log("Share not supported");
            }
        }



        //
        //  Share Screenshot
        //

        private void ShareScreenshot()
        {
            if (Share.isSupported)
            {
                StartCoroutine(doShareScreenshot());
            }
            else
            {
                Log("Share not supported");
            }
        }


        IEnumerator doShareScreenshot()
        {
            yield return new WaitForEndOfFrame();

            // Capture screenshot
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();


            // Share
            Share.Instance.share(
                "Look at this screenshot",
                image
            );

            // Clean up 
            Object.Destroy(image);
        }



        //
        //  Launch Application
        //

        private void LaunchApplication()
        {
            if (Share.isSupported)
            {
                plugins.share.applications.Application app
                    = new plugins.share.applications.Application(
                        "com.instagram.android",
                        "instagram://");

                ApplicationOptions options = new ApplicationOptions();
                options.action = ApplicationOptions.ACTION_VIEW;
                options.data = "http://instagram.com/_u/distriqt";
                options.parameters = "user?username=distriqt";

                if (Share.Instance.Applications.IsInstalled(app))
                {
                    Share.Instance.Applications.Launch(app, options);
                }

                Share.Instance.Applications.OnActivityResult += Applications_OnActivityResult;

                Intent intent = new Intent(Intent.ACTION_VIEW);
                intent.packageName = "com.android.chrome";
                intent.data = "https://distriqt.com";

                Share.Instance.Applications.StartActivity(intent);
            }
            else
            {
                Log("Share not supported");
            }
        }

        private void Applications_OnActivityResult(ApplicationEvent e)
        {
            Log("Applications_OnActivityResult: " + e.resultCode);
        }


        //
        //  Email
        //

        private void SendEmail()
        {
            if (Share.isSupported)
            {
                if (Share.Instance.Email.IsMailSupported)
                {
                    string subject = "Test email from unity";
                    string body = "Some awesome message I want to send";
                    string toRecipients = "unityplugins@distriqt.com";

                    Share.Instance.Email.OnCompose += Email_OnCompose;
                    Share.Instance.Email.OnAttachmentError += Email_OnAttachmentError;

                    bool success = Share.Instance.Email.SendMail(subject, body, toRecipients);

                    Log("SendEmail() = " + success);
                }
                else
                {
                    Log("Email not supported");
                }
            }
            else
            {
                Log("Share not supported");
            }
        }



        private void SendEmailWithOptions()
        {
            StartCoroutine( SendEmailWithOptionsAndScreenShot() );
        }

        private IEnumerator SendEmailWithOptionsAndScreenShot()
        {
            yield return new WaitForEndOfFrame();

            string imageName = "screenshot.png";
            ScreenCapture.CaptureScreenshot(imageName);

            string imageFile = UnityEngine.Application.persistentDataPath + "/" + imageName;

            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            if (Share.isSupported)
            {
                if (Share.Instance.Email.IsMailSupported)
                {
                    string subject = "Test email from unity";
                    string body = "Some awesome message I want to send";
                    string toRecipients = "unityplugins@distriqt.com";
                    string ccRecipients = "distriqt@distriqt.com";
                    string bccRecipients = "airnativeextensions@distriqt.com";

                    Attachment attachment = new Attachment();
                    attachment.nativePath = imageFile;

                    Attachment[] attachments = new Attachment[1];
                    attachments[0] = attachment;

                    bool success = Share.Instance.Email.SendMailWithOptions(
                        subject,
                        body,
                        toRecipients,
                        ccRecipients,
                        bccRecipients,
                        attachments
                        );

                    Log("SendEmail() = " + success);
                }
                else
                {
                    Log("Email not supported");
                }
            }
            else
            {
                Log("Share not supported");
            }
        }


        private void Email_OnCompose(EmailEvent e)
        {
            Log("Email_OnCompose: " + e.details);
        }

        private void Email_OnAttachmentError(EmailEvent e)
        {
            Log("Email_OnAttachmentError: " + e.details);
        }


        //
        //  SMS
        //


        private void SendSMS()
        {
            if (Share.isSupported)
            {
                if (Share.Instance.SMS.IsSMSSupported)
                {
                    SMS sms = new SMS();
                    sms.address = "0444444444";
                    sms.message = "Hello from Unity";

                    Share.Instance.SMS.SendSMSWithUI(sms);
                }
                else
                {
                    Log("SMS not supported");
                }
            }
            else
            {
                Log("Share not supported");
            }

        }


        private void SendSMSAndroid()
        {
#if UNITY_ANDROID
            if (Permission.HasUserAuthorizedPermission("android.permission.SEND_SMS"))
            {
                if (Share.isSupported)
                {
                    if (Share.Instance.SMS.IsSMSSupported)
                    {
                        SMS sms = new SMS();
                        sms.address = "0444444444";
                        sms.message = "Hello from Unity";

                        Share.Instance.SMS.SendSMS(sms);
                    }
                    else
                    {
                        Log("SMS not supported");
                    }
                }
                else
                {
                    Log("Share not supported");
                }
            }
#endif
        }


        private void SMS_OnSMSCancelled(SMSEvent e)
        {
            Log("SMS_OnSMSCancelled");
        }

        private void SMS_OnSMSSentError(SMSEvent e)
        {
            Log("SMS_OnSMSSentError");
        }

        private void SMS_OnSMSSent(SMSEvent e)
        {
            Log("SMS_OnSMSSent");
        }

        private void SMS_OnSMSReceived(SMSEvent e)
        {
            Log("SMS_OnSMSReceived");
        }

        private void SMS_OnSMSNotDelivered(SMSEvent e)
        {
            Log("SMS_OnSMSNotDelivered");
        }

        private void SMS_OnSMSDelivered(SMSEvent e)
        {
            Log("SMS_OnSMSDelivered");
        }
    }

}

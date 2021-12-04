using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;

namespace Commands.Email.Templates
{
    public class EmailTemplates
    {
        private static string EmailTemplateResourcePathRoot => "Commands.Email.Templates.";

        private static string GetEmailTemplateFileContents(string templatePath)
        {
            using var stream = GetEmbeddedResourceStream(EmailTemplateResourcePathRoot + templatePath);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static Stream GetEmbeddedResourceStream(string resourcePath)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
        }

        private static string ReplaceTemplateTokens(string template, IEnumerable<Token> tokens)
        {
            return tokens.Aggregate(template, (current, token) => current.Replace(token.Key, token.Value));
        }

        public Attachment GetLogoAttachment()
        {
            var att = new Attachment(GetEmbeddedResourceStream(EmailTemplateResourcePathRoot + "Images.logo.png"), new ContentType("image/png"));
            if (att.ContentDisposition != null)
            {
                att.ContentDisposition.Inline = true;
                att.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            }

            att.ContentId = "logo";
            att.ContentType.MediaType = "image/png";
            att.ContentType.Name = "logo.png";
            return att;
        }

        private class Token
        {
            private Token(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; }
            public string Value { get; }
        }
    }
}

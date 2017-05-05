﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HwInf.Common.DAL;
using HwInf.ViewModels;
using HwInf.Common;
using System.IO;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.Rendering;
using System.Net.Http.Headers;
using System.Web.Http.Results;
using HwInf.Common.BL;
using log4net;


namespace HwInf.Controllers
{

    [RoutePrefix("print")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PrintController : ApiController
    {
        private readonly IDAL _db;
        private readonly BL _bl;
        private readonly ILog _log = LogManager.GetLogger(typeof(PrintController).Name);

        public PrintController()
        {
            _db = new HwInfContext();
            _bl = new BL(_db);
        }

        public PrintController(IDAL db)
        {
            _db = db;
            _bl = new BL(db);
        }

        /// <summary>
        /// Starts the RunTime Text Template and creates the contract as pdf
        /// </summary>
        /// <param name="guid">Order guid</param>
        /// <returns></returns>
        [Route("{guid}")]
        public HttpResponseMessage GetPrint(Guid guid)
        {
            try
            {
                var order = _bl.GetOrders(guid);
                var rpt = new Contract(order, _bl);
                // Report -> String
                var text = rpt.TransformText();


                // Stream für den DdlReader erzeugen
                MemoryStream stream = CreateMDDLStream(text);
                var errors = new DdlReaderErrors();
                DdlReader rd = new DdlReader(stream, errors);

                // MDDL einlesen
                var doc = rd.ReadDocument();
                stream.Close();
                // MigraDoc Dokument in ein PDF Rendern
                PdfDocumentRenderer pdf = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.None);
                pdf.Document = doc;
                pdf.RenderDocument();

                using (var ms = new MemoryStream())
                {
                    pdf.Save(ms, false);
                    var buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Flush();
                    ms.Read(buffer, 0, (int)ms.Length);

                    // Serve the file to the client

                    var result = new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(buffer)};
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    result.Content.Headers.ContentDisposition.FileName = "Vertrag_"+ order.OrderId + ".pdf";


                    return result;
                }

            }

            catch (Exception ex)
            {
                _log.ErrorFormat("Exception: '{0}'", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError); ;
            }

        }

        /// <summary>
        /// Starts the RunTime Text Template and creates the return-contract as pdf
        /// </summary>
        /// <param name="guid">Order guid</param>
        /// <returns></returns>
        [Route("return/{guid}")]
        public HttpResponseMessage GetReturn(Guid guid)
        {
            try
            {
                var order = _bl.GetOrders(guid);
                var rpt = new ReturnContract(order, _bl);
                // Report -> String
                var text = rpt.TransformText();


                // Stream für den DdlReader erzeugen
                MemoryStream stream = CreateMDDLStream(text);
                var errors = new DdlReaderErrors();
                DdlReader rd = new DdlReader(stream, errors);

                // MDDL einlesen
                var doc = rd.ReadDocument();
                stream.Close();
                // MigraDoc Dokument in ein PDF Rendern
                PdfDocumentRenderer pdf = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.None);
                pdf.Document = doc;
                pdf.RenderDocument();

                using (var ms = new MemoryStream())
                {
                    pdf.Save(ms, false);
                    var buffer = new byte[ms.Length];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Flush();
                    ms.Read(buffer, 0, (int)ms.Length);

                    // Serve the file to the client
                    var result = new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(buffer)};
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    result.Content.Headers.ContentDisposition.FileName = "Rueckgabe" + order.OrderId + ".pdf";

                    return result;
                }

            }

            catch (Exception ex)
            {
                _log.ErrorFormat("Exception: '{0}'", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError); ;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }


        private static MemoryStream CreateMDDLStream(string text)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter sw = new StreamWriter(stream);

            sw.Write(text);

            sw.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

    }
}
﻿using Microsoft.AspNetCore.Http;

namespace System.Web.UI
{
    public class Page
    {
        private Microsoft.AspNetCore.Http.HttpContext context;

        public Page(Microsoft.AspNetCore.Http.HttpContext context)
        {
            this.context = context;
        }

        internal object FindControl(string key)
        {
            throw new NotImplementedException();
        }
        protected virtual void OnPreInit(EventArgs e) { }
    }
}
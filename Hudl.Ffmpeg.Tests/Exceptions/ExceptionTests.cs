using Hudl.FFmpeg.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Hudl.FFmpeg.Tests.Exceptions
{
    public class ExceptionTests
    {
        [Fact]
        public void ExceptionsRaised()
        {
            Assert.Throws<FFmpegProcessingException>(new Action(() => throw new FFmpegProcessingException(1, "foo")));

            Assert.Throws<FFmpegRenderingException>(new Action(() => throw new FFmpegRenderingException(null)));

            Assert.Throws<ForStreamInvalidException>(new Action(() => throw new ForStreamInvalidException(typeof(string) , typeof(string))));

            Assert.Throws<StreamNotFoundException>(new Action(() => throw new StreamNotFoundException(typeof(string))));

            Assert.Throws<StreamNotFoundException>(new Action(() => throw new StreamNotFoundException()));
        }
    }
}

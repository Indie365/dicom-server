﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dicom;
using Microsoft.Health.Dicom.Core.Exceptions;
using Microsoft.Health.Dicom.Core.Features.Retrieve;
using Microsoft.Health.Dicom.Core.Messages.Retrieve;
using Microsoft.Health.Dicom.Tests.Common;
using NSubstitute;
using Xunit;

namespace Microsoft.Health.Dicom.Core.UnitTests.Features.Retrieve
{
    public class DicomRetrieveMetadataHandlerTests
    {
        private readonly IDicomRetrieveMetadataService _dicomRetrieveMetadataService;
        private readonly DicomRetrieveMetadataHandler _dicomRetrieveMetadataHandler;

        public DicomRetrieveMetadataHandlerTests()
        {
            _dicomRetrieveMetadataService = Substitute.For<IDicomRetrieveMetadataService>();
            _dicomRetrieveMetadataHandler = new DicomRetrieveMetadataHandler(_dicomRetrieveMetadataService);
        }

        [Theory]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("345%^&")]
        [InlineData("()")]
        public async Task GivenARequestWithInvalidStudyInstanceIdentifier_WhenHandlerIsExecuted_ThenDicomInvalidIdentifierExceptionIsThrown(string studyInstanceUid)
        {
            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(studyInstanceUid);
            var ex = await Assert.ThrowsAsync<DicomInvalidIdentifierException>(() => _dicomRetrieveMetadataHandler.Handle(request, CancellationToken.None));

            Assert.Equal($"DICOM Identifier 'StudyInstanceUid' value '{studyInstanceUid.Trim()}' is invalid. Value length should not exceed the maximum length of 64 characters. Value should contain characters in '0'-'9' and '.'. Each component must start with non-zero number.", ex.Message);
        }

        [Theory]
        [InlineData("aaaa-bbbb", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("aaaa-bbbb", " ")]
        [InlineData("aaaa-bbbb", "345%^&")]
        [InlineData("aaaa-bbbb", "aaaa-bbbb")]
        public async Task GivenARequestWithInvalidStudyIdentifier_WhenRetrievingSeriesMetadata_ThenDicomInvalidIdentifierExceptionIsThrown(string studyInstanceUid, string seriesInstanceUid)
        {
            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(studyInstanceUid, seriesInstanceUid);
            var ex = await Assert.ThrowsAsync<DicomInvalidIdentifierException>(() => _dicomRetrieveMetadataHandler.Handle(request, CancellationToken.None));

            Assert.Equal($"DICOM Identifier 'StudyInstanceUid' value '{studyInstanceUid.Trim()}' is invalid. Value length should not exceed the maximum length of 64 characters. Value should contain characters in '0'-'9' and '.'. Each component must start with non-zero number.", ex.Message);
        }

        [Theory]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("345%^&")]
        [InlineData("aaaa-bbbb")]
        [InlineData("()")]
        public async Task GivenARequestWithInvalidSeriesIdentifier_WhenRetrievingSeriesMetadata_ThenDicomInvalidIdentifierExceptionIsThrown(string seriesInstanceUid)
        {
            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(TestUidGenerator.Generate(), seriesInstanceUid);
            var ex = await Assert.ThrowsAsync<DicomInvalidIdentifierException>(() => _dicomRetrieveMetadataHandler.Handle(request, CancellationToken.None));

            Assert.Equal($"DICOM Identifier 'SeriesInstanceUid' value '{seriesInstanceUid.Trim()}' is invalid. Value length should not exceed the maximum length of 64 characters. Value should contain characters in '0'-'9' and '.'. Each component must start with non-zero number.", ex.Message);
        }

        [Theory]
        [InlineData("aaaa-bbbb1", "aaaa-bbbb2", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("aaaa-bbbb1", "aaaa-bbbb2", "345%^&")]
        [InlineData("aaaa-bbbb1", "aaaa-bbbb2", "aaaa-bbbb2")]
        [InlineData("aaaa-bbbb1", "aaaa-bbbb2", "aaaa-bbbb1")]
        public async Task GivenARequestWithInvalidStudyAndSeriesInstanceIdentifier_WhenRetrievingInstanceMetadata_ThenDicomInvalidIdentifierExceptionIsThrown(string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid)
        {
            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(studyInstanceUid, seriesInstanceUid, sopInstanceUid);
            var ex = await Assert.ThrowsAsync<DicomInvalidIdentifierException>(() => _dicomRetrieveMetadataHandler.Handle(request, CancellationToken.None));

            Assert.Equal($"DICOM Identifier 'StudyInstanceUid' value '{studyInstanceUid.Trim()}' is invalid. Value length should not exceed the maximum length of 64 characters. Value should contain characters in '0'-'9' and '.'. Each component must start with non-zero number.", ex.Message);
        }

        [Theory]
        [InlineData("aaaa-bbbb2", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("aaaa-bbbb2", "345%^&")]
        [InlineData("aaaa-bbbb2", "aaaa-bbbb2")]
        [InlineData("aaaa-bbbb2", " ")]
        public async Task GivenARequestWithInvalidSeriesInstanceIdentifier_WhenRetrievingInstanceMetadata_ThenDicomInvalidIdentifierExceptionIsThrown(string seriesInstanceUid, string sopInstanceUid)
        {
            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(TestUidGenerator.Generate(), seriesInstanceUid, sopInstanceUid);
            var ex = await Assert.ThrowsAsync<DicomInvalidIdentifierException>(() => _dicomRetrieveMetadataHandler.Handle(request, CancellationToken.None));

            Assert.Equal($"DICOM Identifier 'SeriesInstanceUid' value '{seriesInstanceUid.Trim()}' is invalid. Value length should not exceed the maximum length of 64 characters. Value should contain characters in '0'-'9' and '.'. Each component must start with non-zero number.", ex.Message);
        }

        [Theory]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("345%^&")]
        [InlineData("aaaa-bbbb")]
        [InlineData("()")]
        public async Task GivenARequestWithInvalidSopInstanceIdentifier_WhenRetrievingInstanceMetadata_ThenDicomInvalidIdentifierExceptionIsThrown(string sopInstanceUid)
        {
            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(TestUidGenerator.Generate(), TestUidGenerator.Generate(), sopInstanceUid);
            var ex = await Assert.ThrowsAsync<DicomInvalidIdentifierException>(() => _dicomRetrieveMetadataHandler.Handle(request, CancellationToken.None));

            Assert.Equal($"DICOM Identifier 'SopInstanceUid' value '{sopInstanceUid.Trim()}' is invalid. Value length should not exceed the maximum length of 64 characters. Value should contain characters in '0'-'9' and '.'. Each component must start with non-zero number.", ex.Message);
        }

        [Fact]
        public async Task GivenARequestWithValidInstanceIdentifier_WhenRetrievingStudyInstanceMetadata_ThenResponseMetadataIsReturnedSuccessfully()
        {
            string studyInstanceUid = TestUidGenerator.Generate();

            DicomRetrieveMetadataResponse response = SetupRetrieveMetadataResponse();
            _dicomRetrieveMetadataService.RetrieveStudyInstanceMetadataAsync(studyInstanceUid).Returns(response);

            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(studyInstanceUid);
            await ValidateRetrieveMetadataResponse(response, request);
        }

        [Fact]
        public async Task GivenARequestWithValidInstanceIdentifier_WhenRetrievingSeriesInstanceMetadata_ThenResponseMetadataIsReturnedSuccessfully()
        {
            string studyInstanceUid = TestUidGenerator.Generate();
            string seriesInstanceUid = TestUidGenerator.Generate();

            DicomRetrieveMetadataResponse response = SetupRetrieveMetadataResponse();
            _dicomRetrieveMetadataService.RetrieveSeriesInstanceMetadataAsync(studyInstanceUid, seriesInstanceUid).Returns(response);

            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(studyInstanceUid, seriesInstanceUid);
            await ValidateRetrieveMetadataResponse(response, request);
        }

        [Fact]
        public async Task GivenARequestWithValidInstanceIdentifier_WhenRetrievingSopInstanceMetadata_ThenResponseMetadataIsReturnedSuccessfully()
        {
            string studyInstanceUid = TestUidGenerator.Generate();
            string seriesInstanceUid = TestUidGenerator.Generate();
            string sopInstanceUid = TestUidGenerator.Generate();

            DicomRetrieveMetadataResponse response = SetupRetrieveMetadataResponse();
            _dicomRetrieveMetadataService.RetrieveSopInstanceMetadataAsync(studyInstanceUid, seriesInstanceUid, sopInstanceUid).Returns(response);

            DicomRetrieveMetadataRequest request = new DicomRetrieveMetadataRequest(studyInstanceUid, seriesInstanceUid, sopInstanceUid);
            await ValidateRetrieveMetadataResponse(response, request);
        }

        private static DicomRetrieveMetadataResponse SetupRetrieveMetadataResponse()
        {
            return new DicomRetrieveMetadataResponse(
                HttpStatusCode.OK,
                new List<DicomDataset> { new DicomDataset() });
        }

        private async Task ValidateRetrieveMetadataResponse(DicomRetrieveMetadataResponse response, DicomRetrieveMetadataRequest request)
        {
            DicomRetrieveMetadataResponse actualResponse = await _dicomRetrieveMetadataHandler.Handle(request, CancellationToken.None);
            Assert.Same(response, actualResponse);
        }
    }
}

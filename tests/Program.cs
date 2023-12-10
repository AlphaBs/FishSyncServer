#if !TEST_SDK

using AlphabetUpdateServer.Tests;

var tester = new BucketTest();
await tester.Sync_success();

#endif
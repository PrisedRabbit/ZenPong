#include <ifaddrs.h>
#include <arpa/inet.h>

#import "IPAddress.h"
@implementation IPAddressDelegate

- (id)init
{
    self = [super init];
    return self;
}

- (NSString *)getAddress {
    NSString *address = @"error";
    struct ifaddrs *interfaces = NULL;
    struct ifaddrs *temp_addr = NULL;
    int success = 0;
    success = getifaddrs(&interfaces);
    if (success == 0) {
        temp_addr = interfaces;
        while(temp_addr != NULL) {
            if(temp_addr->ifa_addr->sa_family == AF_INET) {
                if([[NSString stringWithUTF8String:temp_addr->ifa_name] isEqualToString:@"en0"]) {
                    address = [NSString stringWithUTF8String:inet_ntoa(((struct sockaddr_in *)temp_addr->ifa_addr)->sin_addr)];
                }
            }
            temp_addr = temp_addr->ifa_next;
        }
    }
    // Free memory
    freeifaddrs(interfaces);
    return address;
}
@end

static IPAddressDelegate* delegateObject = nil;

char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

const char * getLocalWifiIpAddress()
{
    if (delegateObject == nil)
        delegateObject = [[IPAddressDelegate alloc] init];
    
    return MakeStringCopy([[delegateObject getAddress] UTF8String]);
}
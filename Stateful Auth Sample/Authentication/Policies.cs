using Microsoft.AspNetCore.Authorization;

namespace Stateful_Auth_Sample.Authentication
{
    public static class Policies
    {

        // Policies need to be const strings so they can be used in Authorize method attribute.

        public const string CanAddUsers = nameof(CanAddUsers);
        public const string CanDeleteUsers = nameof(CanDeleteUsers);

        public const string CanAddNotes = nameof(CanAddNotes);
        public const string CanDeleteNotes = nameof(CanDeleteNotes);
        public const string CanReadNotes = nameof(CanReadNotes);
        public const string CanUpdateNotes = nameof(CanUpdateNotes);

        public static void AddPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(CanReadNotes, policyOptions => policyOptions.RequireRole(Roles.Creator, Roles.Reader));

            options.AddPolicy(CanAddNotes, policyOptions => policyOptions.RequireRole(Roles.Creator));
            options.AddPolicy(CanDeleteNotes, policyOptions => policyOptions.RequireRole(Roles.Creator));
            options.AddPolicy(CanUpdateNotes, policyOptions => policyOptions.RequireRole(Roles.Creator));

            options.AddPolicy(CanAddUsers, policyOptions => policyOptions.RequireRole(Roles.Creator));
            options.AddPolicy(CanDeleteUsers, policyOptions => policyOptions.RequireRole(Roles.Creator));
        }
    }
}

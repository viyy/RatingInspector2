using System.Collections.Generic;
using Models;

namespace Interfaces
{
    public interface IProfileManager : IGroupListProvider
    {
        IEnumerable<Profile> GetProfiles(string needle = null);
        void DeleteProfile(int id);
        void SaveProfile(Profile profile);
        void MergeProfiles(Profile targetProfile, RcfProfile rcfProfile, FideProfile fideProfile);
        IEnumerable<RcfProfile> GetRcfProfiles();
        RcfProfile SearchRcfProfile(int rcfId);
        IEnumerable<FideProfile> GetFideProfiles();
        FideProfile SearchFideProfile(int fideId);
    }
}
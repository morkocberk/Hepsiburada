using Data.Abstract;
using Data.Entity;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Concrete
{
    public class CampaignDataDal : Repository<Campaign>, ICampaignDataDal
    {
    }
}

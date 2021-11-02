using ProjectWebjar.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebjar
{
    public interface ICronJob
    {
        public Task DeletedHeavyPics();
    }
    public class CronJob: ICronJob
    {
        private readonly IPicsRepository _picsRepository;

        public CronJob(IPicsRepository picsRepository)
        {
            _picsRepository = picsRepository;
        }

        public async Task DeletedHeavyPics()
        {
            var HeavyPics = await _picsRepository.FindPicsGraterThan(100);
            foreach (var pic in HeavyPics)
            {
                var CurrentImage = Path.Combine(AppContext.BaseDirectory, "uploads", pic.PicturePath);

                if (File.Exists(CurrentImage))
                {
                    File.Delete(CurrentImage);
                }

                await _picsRepository.Delete(pic);
            }
        }
    }
}
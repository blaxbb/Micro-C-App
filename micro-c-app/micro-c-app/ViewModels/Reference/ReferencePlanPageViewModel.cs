using MicroCLib.Models.Reference;
using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.ViewModels.Reference
{
    public class ReferencePlanPageViewModel : BaseViewModel
    {
        private List<PlanReference> plans;

        public List<PlanReference> Plans { get => plans; set => SetProperty(ref plans, value); }

        public ReferencePlanPageViewModel()
        {
            Plans = new List<PlanReference>();
        }
    }
}

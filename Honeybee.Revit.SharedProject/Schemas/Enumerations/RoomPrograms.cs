using System.Collections.Generic;
using Honeybee.Core;
// ReSharper disable UnusedMember.Global

namespace Honeybee.Revit.Schemas.Enumerations
{
    public class RoomPrograms : Enumeration
    {
        public static readonly RoomPrograms Attic = new RoomPrograms(0, "Attic");
        public static readonly RoomPrograms BreakRoom = new RoomPrograms(1, "BreakRoom");
        public static readonly RoomPrograms Classroom = new RoomPrograms(2, "Classroom");
        public static readonly RoomPrograms ClosedOffice = new RoomPrograms(3, "ClosedOffice");
        public static readonly RoomPrograms Conference = new RoomPrograms(4, "Conference");
        public static readonly RoomPrograms Corridor = new RoomPrograms(5, "Corridor");
        public static readonly RoomPrograms Dining = new RoomPrograms(6, "Dining");
        public static readonly RoomPrograms ElecMechRoom = new RoomPrograms(7, "Elec/MechRoom");
        public static readonly RoomPrograms ItRoom = new RoomPrograms(8, "IT_Room");
        public static readonly RoomPrograms Lobby = new RoomPrograms(9, "Lobby");
        public static readonly RoomPrograms OfficeLargeDataCenter = new RoomPrograms(10, "OfficeLarge Data Center");
        public static readonly RoomPrograms OfficeLargeMainDataCenter = new RoomPrograms(11, "OfficeLarge Main Data Center");
        public static readonly RoomPrograms OpenOffice = new RoomPrograms(12, "OpenOffice");
        public static readonly RoomPrograms PrintRoom = new RoomPrograms(13, "PrintRoom");
        public static readonly RoomPrograms Restroom = new RoomPrograms(14, "Restroom");
        public static readonly RoomPrograms Stair = new RoomPrograms(15, "Stair");
        public static readonly RoomPrograms Storage = new RoomPrograms(16, "Storage");
        public static readonly RoomPrograms Vending = new RoomPrograms(17, "Vending");
        public static readonly RoomPrograms PointOfSale = new RoomPrograms(18, "Point_of_Sale");
        public static readonly RoomPrograms Retail = new RoomPrograms(19, "Retail");
        public static readonly RoomPrograms BreakRoom2 = new RoomPrograms(20, "Breakroom");

        public static readonly RoomPrograms BreakRoom3 = new RoomPrograms(21, "Break Room");
        public static readonly RoomPrograms Cell = new RoomPrograms(22, "Cell");
        public static readonly RoomPrograms Courtroom = new RoomPrograms(23, "Courtroom");
        public static readonly RoomPrograms CourtroomWaiting = new RoomPrograms(24, "Courtroom Waiting");
        public static readonly RoomPrograms ElevatorLobby = new RoomPrograms(25, "Elevator Lobby");
        public static readonly RoomPrograms ElevatorShaft = new RoomPrograms(26, "Elevator Shaft");
        public static readonly RoomPrograms EntranceLobby = new RoomPrograms(27, "Entrance Lobby");
        public static readonly RoomPrograms JudgesChamber = new RoomPrograms(28, "Judges Chamber");
        public static readonly RoomPrograms JuryAssembly = new RoomPrograms(29, "Jury Assembly");
        public static readonly RoomPrograms JuryDeliberation = new RoomPrograms(30, "Jury Deliberation");
        public static readonly RoomPrograms Library = new RoomPrograms(31, "Library");
        public static readonly RoomPrograms Office = new RoomPrograms(32, "Office");
        public static readonly RoomPrograms Parking = new RoomPrograms(33, "Parking");
        public static readonly RoomPrograms Plenum = new RoomPrograms(34, "Plenum");
        public static readonly RoomPrograms Restrooms = new RoomPrograms(35, "Restrooms");
        public static readonly RoomPrograms SecurityScreening = new RoomPrograms(36, "Security Screening");
        public static readonly RoomPrograms ServiceShaft = new RoomPrograms(37, "Service Shaft");
        public static readonly RoomPrograms Stairs = new RoomPrograms(38, "Stairs");
        public static readonly RoomPrograms Utility = new RoomPrograms(39, "Utility");

        public static readonly RoomPrograms Kitchen = new RoomPrograms(40, "Kitchen");

        public static readonly RoomPrograms Apartment = new RoomPrograms(41, "Apartment");

        public static readonly RoomPrograms Basement = new RoomPrograms(42, "Basement");
        public static readonly RoomPrograms ErExam = new RoomPrograms(43, "ER_Exam");
        public static readonly RoomPrograms ErNurseStn = new RoomPrograms(44, "ER_NurseStn");
        public static readonly RoomPrograms ErTrauma = new RoomPrograms(45, "ER_Trauma");
        public static readonly RoomPrograms ErTriage = new RoomPrograms(46, "ER_Triage");
        public static readonly RoomPrograms HospitalOffice = new RoomPrograms(47, "HospitalOffice");
        public static readonly RoomPrograms IcuNurseStn = new RoomPrograms(48, "ICU_NurseStn");
        public static readonly RoomPrograms IcuOpen = new RoomPrograms(49, "ICU_Open");
        public static readonly RoomPrograms IcuPatRm = new RoomPrograms(50, "ICU_PatRm");
        public static readonly RoomPrograms Lab = new RoomPrograms(51, "Lab");
        public static readonly RoomPrograms NurseStn = new RoomPrograms(52, "NurseStn");
        public static readonly RoomPrograms Or = new RoomPrograms(53, "OR");
        public static readonly RoomPrograms PatCorridor = new RoomPrograms(54, "PatCorridor");
        public static readonly RoomPrograms PatRoom = new RoomPrograms(55, "PatRoom");
        public static readonly RoomPrograms PhysTherapy = new RoomPrograms(56, "PhysTherapy");
        public static readonly RoomPrograms Radiology = new RoomPrograms(57, "Radiology");

        public static readonly RoomPrograms EquipmentCorridor = new RoomPrograms(58, "Equipment corridor");
        public static readonly RoomPrograms LabWithFumeHood = new RoomPrograms(59, "Lab with fume hood");
        public static readonly RoomPrograms OpenLab = new RoomPrograms(60, "Open lab");

        public static readonly RoomPrograms StandaloneDataCenter = new RoomPrograms(61, "StandaloneDataCenter");

        public static readonly RoomPrograms Banquet = new RoomPrograms(62, "Banquet");
        public static readonly RoomPrograms Cafe = new RoomPrograms(63, "Cafe");
        public static readonly RoomPrograms GuestRoom = new RoomPrograms(64, "GuestRoom");
        public static readonly RoomPrograms GuestRoom2 = new RoomPrograms(65, "GuestRoom2");
        public static readonly RoomPrograms GuestRoom3 = new RoomPrograms(66, "GuestRoom3");
        public static readonly RoomPrograms GuestRoom4 = new RoomPrograms(67, "GuestRoom4");
        public static readonly RoomPrograms Laundry = new RoomPrograms(68, "Laundry");
        public static readonly RoomPrograms Mechanical = new RoomPrograms(69, "Mechanical");

        public static readonly RoomPrograms Anesthesia = new RoomPrograms(70, "Anesthesia");
        public static readonly RoomPrograms BioHazard = new RoomPrograms(71, "BioHazard");
        public static readonly RoomPrograms CleanWork = new RoomPrograms(72, "CleanWork");
        public static readonly RoomPrograms DressingRoom = new RoomPrograms(73, "DressingRoom");
        public static readonly RoomPrograms ElevatorPumpRoom = new RoomPrograms(74, "ElevatorPumpRoom");
        public static readonly RoomPrograms Exam = new RoomPrograms(75, "Exam");
        public static readonly RoomPrograms Hall = new RoomPrograms(76, "Hall");
        public static readonly RoomPrograms Janitor = new RoomPrograms(77, "Janitor");
        public static readonly RoomPrograms LockerRoom = new RoomPrograms(78, "LockerRoom");
        public static readonly RoomPrograms Lounge = new RoomPrograms(79, "Lounge");
        public static readonly RoomPrograms Mri = new RoomPrograms(80, "MRI");
        public static readonly RoomPrograms MriControl = new RoomPrograms(81, "MRI_Control");
        public static readonly RoomPrograms MedGas = new RoomPrograms(82, "MedGas");
        public static readonly RoomPrograms NurseStation = new RoomPrograms(83, "NurseStation");
        public static readonly RoomPrograms Pacu = new RoomPrograms(84, "PACU");
        public static readonly RoomPrograms PhysicalTherapy = new RoomPrograms(85, "PhysicalTherapy");
        public static readonly RoomPrograms PreOp = new RoomPrograms(86, "PreOp");
        public static readonly RoomPrograms ProcedureRoom = new RoomPrograms(87, "ProcedureRoom");
        public static readonly RoomPrograms Reception = new RoomPrograms(88, "Reception");
        public static readonly RoomPrograms SoilWork = new RoomPrograms(89, "Soil Work");
        public static readonly RoomPrograms Toilet = new RoomPrograms(90, "Toilet");
        public static readonly RoomPrograms Undeveloped = new RoomPrograms(91, "Undeveloped");
        public static readonly RoomPrograms Xray = new RoomPrograms(92, "Xray");

        public static readonly RoomPrograms Cafeteria = new RoomPrograms(93, "Cafeteria");
        public static readonly RoomPrograms ComputerRoom = new RoomPrograms(94, "ComputerRoom");
        public static readonly RoomPrograms Gym = new RoomPrograms(95, "Gym");

        public static readonly RoomPrograms BackSpace = new RoomPrograms(96, "Back_Space");
        public static readonly RoomPrograms CoreRetail = new RoomPrograms(97, "Core_Retail");
        public static readonly RoomPrograms Entry = new RoomPrograms(98, "Entry");
        public static readonly RoomPrograms FrontRetail = new RoomPrograms(99, "Front_Retail");

        public static readonly RoomPrograms Auditorium = new RoomPrograms(100, "Auditorium");
        
        public static readonly RoomPrograms ElevatorCore = new RoomPrograms(101, "ElevatorCore");
        public static readonly RoomPrograms Exercise = new RoomPrograms(102, "Exercise");
        public static readonly RoomPrograms GuestLounge = new RoomPrograms(103, "GuestLounge");
        public static readonly RoomPrograms GuestRoomOcc = new RoomPrograms(104, "GuestRoomOcc");
        public static readonly RoomPrograms GuestRoomVac = new RoomPrograms(105, "GuestRoomVac");
        public static readonly RoomPrograms Meeting = new RoomPrograms(106, "Meeting");
        public static readonly RoomPrograms PublicRestroom = new RoomPrograms(107, "PublicRestroom");
        public static readonly RoomPrograms StaffLounge = new RoomPrograms(108, "StaffLounge");

        public static readonly RoomPrograms Type0A = new RoomPrograms(109, "Type 0A");
        public static readonly RoomPrograms Type0B = new RoomPrograms(110, "Type 0B");
        public static readonly RoomPrograms Type1 = new RoomPrograms(111, "Type 1");
        public static readonly RoomPrograms Type2 = new RoomPrograms(112, "Type 2");
        public static readonly RoomPrograms Type3 = new RoomPrograms(113, "Type 3");

        public static readonly RoomPrograms Bakery = new RoomPrograms(114, "Bakery");
        public static readonly RoomPrograms Deli = new RoomPrograms(115, "Deli");
        public static readonly RoomPrograms DryStorage = new RoomPrograms(116, "DryStorage");
        public static readonly RoomPrograms Produce = new RoomPrograms(117, "Produce");
        public static readonly RoomPrograms Sales = new RoomPrograms(118, "Sales");
        public static readonly RoomPrograms Vestibule = new RoomPrograms(119, "Vestibule");

        public static readonly RoomPrograms Bulk = new RoomPrograms(120, "Bulk");
        public static readonly RoomPrograms Fine = new RoomPrograms(121, "Fine");

        public static readonly RoomPrograms GymAudience = new RoomPrograms(122, "Gym - audience");



        public RoomPrograms()
        {
        }

        private RoomPrograms(int value, string displayName) : base(value, displayName)
        {
        }

        public IEnumerable<RoomPrograms> GetAll()
        {
            return GetAll<RoomPrograms>();
        }
    }
}

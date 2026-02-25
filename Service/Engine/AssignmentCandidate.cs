using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.Engine;

public class AssignmentCandidate()
{
    public CourseOffering CourseOffering;
    public Professor Professor;
    public Room Room;
    public TimeSlot TimeSlot;

    public int Score;



}
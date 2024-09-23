using System;
using UnityEngine.Events;

[Serializable]
public class Task
{
	public TaskType taskType;
	public Villager villager;
	public string text;
	public string name;
	public string answer;
	public string hideString;
	public int hideInt;
	public UnityAction<Task> CheckConditionsMet;

	public Task()
	{
	}

	public Task(Task _task)
	{
		taskType = _task.taskType;
		text = _task.text;
		name = _task.name;
		answer = _task.answer;
		villager = _task.villager;
	}


	public Task(TaskType _taskType, string _text, string _name, string _answer, Villager _villager)
	{
		taskType = _taskType;
		text = _text;
		name = _name;
		answer = _answer;
		villager = _villager;
	}
}
public enum TaskType
{
	bring,
	make,
	observe
}
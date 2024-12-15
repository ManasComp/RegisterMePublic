#region

using FluentAssertions;
using NUnit.Framework;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.UnitTests;

public class GroupIdTest
{
    [Test]
    public void Test_That_Payments_Works_For_SCHK()
    {
        HashSet<string> hashSet =
        [
            "7",
            "2",
            "3",
            "8",
            "1",
            "4",
            "13a",
            "5",
            "13c",
            "17",
            "11",
            "15",
            "16",
            "12",
            "14",
            "13b",
            "6",
            "9",
            "10"
        ];

        List<string> shouldBe =
        [
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13a",
            "13b",
            "13c",
            "14",
            "15",
            "16",
            "17"
        ];

        List<string> groupsId = hashSet.OrderBy(x => new GroupId(x)).ToList();

        groupsId.SequenceEqual(shouldBe).Should().BeTrue();
    }
}
